﻿// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using FlowtideDotNet.Storage.FileCache.Internal;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FlowtideDotNet.Storage.FileCache
{
    internal class FileCache : IDisposable
    {
        private readonly long cacheSegmentSize;
        private readonly object m_lock = new object();
        private readonly FileCacheOptions fileCacheOptions;
        private readonly string fileName;
        private readonly int m_sectorSize;
        Dictionary<long, LinkedListNode<Allocation>> allocatedPages = new Dictionary<long, LinkedListNode<Allocation>>();
        LinkedList<Allocation> memoryNodes = new LinkedList<Allocation>();

        private Dictionary<int, FileCacheSegmentWriter> segmentWriters = new Dictionary<int, FileCacheSegmentWriter>();
        private bool disposedValue;

        public FileCache(FileCacheOptions fileCacheOptions, string fileName)
        {
            cacheSegmentSize = fileCacheOptions.SegmentSize;
            this.fileCacheOptions = fileCacheOptions;
            this.fileName = fileName;

            m_sectorSize = GetSectorSize();
        }

        private int GetSectorSize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (WindowsNative.GetDiskFreeSpace(GenerateFileName(0), out uint lpSectorsPerCluster,
                                        out uint sectorSize,
                                        out uint lpNumberOfFreeClusters,
                                        out uint lpTotalNumberOfClusters))
                {
                    return (int)sectorSize;
                }
                return 512;
            }
            return 512;
        }

        public void Free(in long pageKey)
        {
            lock (m_lock)
            {
                Free_NoLock(pageKey);
            }
        }

        private string GenerateFileName(int fileNumber)
        {
            return Path.Combine(fileCacheOptions.DirectoryPath, $"fileCache.{fileName}.{fileNumber}.data");
        }

        private void CheckifSegmentCanBeRemoved(LinkedListNode<Allocation> node)
        {
            if (((node.Previous != null && node.Previous.ValueRef.fileNumber != node.ValueRef.fileNumber) || node.Previous == null) &&
                        (node.Next == null || node.ValueRef.fileNumber != node.Next.ValueRef.fileNumber))
            {
                memoryNodes.Remove(node);
                if (segmentWriters.TryGetValue(node.ValueRef.fileNumber, out var segment))
                {
                    segment.Dispose();
                    segmentWriters.Remove(node.ValueRef.fileNumber);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void Free_NoLock(in long pageKey)
        {
            Debug.Assert(Monitor.IsEntered(m_lock));
            if (allocatedPages.TryGetValue(pageKey, out var node))
            {
                if (node.Next != null && node.Next.ValueRef.pageKey == null && node.Next.ValueRef.fileNumber == node.ValueRef.fileNumber)
                {
                    node.ValueRef.pageKey = null;
                    node.ValueRef.allocatedSize = node.ValueRef.allocatedSize + node.Next.ValueRef.allocatedSize;
                    // remove the next node since we took its size
                    memoryNodes.Remove(node.Next);
                    allocatedPages.Remove(pageKey);

                    // Check if we can remove this segment
                    CheckifSegmentCanBeRemoved(node);
                }
                // Check if the previous node exist and it is free, if so, give the size of this node to the previous node.
                // They must also be on the same file segment to be able to be merged.
                if (node.Previous != null && node.Previous.ValueRef.pageKey == null && node.Previous.ValueRef.fileNumber == node.ValueRef.fileNumber)
                {
                    var previousNode = node.Previous;
                    node.Previous.ValueRef.allocatedSize = node.Previous.ValueRef.allocatedSize + node.ValueRef.allocatedSize;
                    
                    // remove the node
                    if (node.List != null)
                    {
                        memoryNodes.Remove(node);
                    }
                    allocatedPages.Remove(pageKey);

                    CheckifSegmentCanBeRemoved(previousNode);
                }
                // Check if this is the only free page in a segment, this allows deletion of the entire segment.
                else if (node.List != null &&
                        ((node.Previous != null && node.Previous.ValueRef.fileNumber != node.ValueRef.fileNumber) || node.Previous == null) &&
                        (node.Next == null || node.ValueRef.fileNumber != node.Next.ValueRef.fileNumber))
                {
                    // Remove the node completely
                    if (node.List != null)
                    {
                        memoryNodes.Remove(node);
                    }
                    
                    allocatedPages.Remove(pageKey);
                    // Schedule a remove file task for that segment
                    if (segmentWriters.TryGetValue(node.ValueRef.fileNumber, out var segment))
                    {
                        segment.Dispose();
                        segmentWriters.Remove(node.ValueRef.fileNumber);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                // Check if there is a free node after this node
                else
                {
                    // Only free this allocation
                    node.ValueRef.pageKey = null;
                    allocatedPages.Remove(pageKey);
                }
            }
        }

        public void Allocate(long pageKey, int size)
        {
            lock (m_lock)
            {
                Allocate_NoLock(pageKey, size);
            }
        }

        private void Allocate_NoLock(long pageKey, int size)
        {
            Debug.Assert(Monitor.IsEntered(m_lock));

            var allocationSize = UpperPowerOfTwo(size);

            // If there are no memory nodes, initialize them and a segment file.
            if (memoryNodes.Count == 0)
            {
                segmentWriters.Add(0, new FileCacheSegmentWriter(GenerateFileName(0), fileCacheOptions));
                memoryNodes.AddLast(new Allocation()
                {
                    fileNumber = 0,
                    pageKey = null,
                    position = 0,
                    allocatedSize = cacheSegmentSize,
                    size = 0
                });
            }

            var iteratorNode = memoryNodes.First;

            while (true)
            {
                if (iteratorNode.ValueRef.pageKey == null && iteratorNode.ValueRef.allocatedSize >= allocationSize)
                {
                    break;
                }
                if (iteratorNode.Next != null)
                {
                    iteratorNode = iteratorNode.Next;
                }
                else
                {
                    break;
                }
            }
            if (iteratorNode.ValueRef.pageKey == null && iteratorNode.ValueRef.allocatedSize >= allocationSize)
            {
                // Allocate this page
                iteratorNode.ValueRef.pageKey = pageKey;
                iteratorNode.ValueRef.size = size;

                allocatedPages.Add(pageKey, iteratorNode);

                var newNode = new LinkedListNode<Allocation>(new Allocation()
                {
                    pageKey = null,
                    position = iteratorNode.ValueRef.position + allocationSize,
                    allocatedSize = iteratorNode.ValueRef.allocatedSize - allocationSize
                });
                iteratorNode.ValueRef.allocatedSize = allocationSize;

                memoryNodes.AddAfter(iteratorNode, newNode);
            }
            else if (iteratorNode.ValueRef.allocatedSize < allocationSize)
            {
                // Create a new node for a new segment allocated for the page key
                var allocatedNode = new LinkedListNode<Allocation>(new Allocation()
                {
                    pageKey = pageKey,
                    position = 0,
                    allocatedSize = allocationSize,
                    fileNumber = iteratorNode.ValueRef.fileNumber + 1,
                    size = size
                });
                // Create a new tail node for the new segment that contains the rest
                var tailNode = new LinkedListNode<Allocation>(new Allocation()
                {
                    pageKey = null,
                    position = allocationSize,
                    fileNumber = allocatedNode.ValueRef.fileNumber,
                    allocatedSize = cacheSegmentSize - allocationSize
                });
                iteratorNode.ValueRef.fileNumber = iteratorNode.ValueRef.fileNumber++;
                memoryNodes.AddAfter(iteratorNode, allocatedNode);
                memoryNodes.AddAfter(allocatedNode, tailNode);

                allocatedPages.Add(pageKey, allocatedNode);

                segmentWriters.Add(allocatedNode.ValueRef.fileNumber, new FileCacheSegmentWriter(GenerateFileName(allocatedNode.ValueRef.fileNumber), fileCacheOptions));
            }
            else
            {
                throw new Exception("Unexpected error");
            }
        }

        /// <summary>
        /// Write data for a page key to storage
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void WriteAsync(long pageKey, byte[] data)
        {
            long position = 0;
            FileCacheSegmentWriter? segmentWriter = null;
            lock (m_lock)
            {
                if (allocatedPages.TryGetValue(pageKey, out var node))
                {
                    // Check if the current node has enough size
                    if (node.ValueRef.allocatedSize >= data.Length)
                    {
                        if (segmentWriters.TryGetValue(node.ValueRef.fileNumber, out segmentWriter))
                        {
                            position = node.ValueRef.position;
                            node.ValueRef.size = data.Length;
                        }
                    }
                    else
                    {
                        // Free the previous page
                        Free(pageKey);
                        // Create a new allocation
                        Allocate_NoLock(pageKey, data.Length);
                        if (allocatedPages.TryGetValue(pageKey, out var newNode))
                        {
                            if (segmentWriters.TryGetValue(newNode.ValueRef.fileNumber, out segmentWriter))
                            {
                                position = newNode.ValueRef.position;
                                newNode.ValueRef.size = data.Length;
                            }
                        }
                    }
                }
                else
                {
                    Allocate_NoLock(pageKey, data.Length);
                    if (allocatedPages.TryGetValue(pageKey, out var newNode))
                    {
                        if (segmentWriters.TryGetValue(newNode.ValueRef.fileNumber, out segmentWriter))
                        {
                            position = newNode.ValueRef.position;
                            newNode.ValueRef.size = data.Length;
                        }
                    }
                }
            }

            if (segmentWriter == null)
            {
                throw new Exception();
            }

            segmentWriter.Write(position, data);
        }

        public bool Exists(long pageKey)
        {
            lock (m_lock)
            {
                return allocatedPages.ContainsKey(pageKey);
            }
        }

        public byte[] Read(long pageKey)
        {
            FileCacheSegmentWriter? segmentWriter = default;
            long position = 0;
            int size = 0;
            lock (m_lock)
            {
                if (allocatedPages.TryGetValue(pageKey, out var node))
                {
                    if (segmentWriters.TryGetValue(node.ValueRef.fileNumber, out var segment))
                    {
                        position = node.ValueRef.position;
                        size = node.ValueRef.size;
                        segmentWriter = segment;
                        //return await segment.Read(node.ValueRef.position, node.ValueRef.size).ConfigureAwait(false);
                    }
                }
            }

            if (segmentWriter != null)
            {
                return segmentWriter.Read(position, size);
            }
            //if (allocatedPages.TryGetValue(pageKey, out var node))
            //{
            //    if (segmentWriters.TryGetValue(node.ValueRef.fileNumber, out var segment))
            //    {
            //        return await segment.Read(node.ValueRef.position, node.ValueRef.size).ConfigureAwait(false);
            //    }
            //}
            throw new Exception();
        }

        private int UpperPowerOfTwo(int v)
        {
            // sector size is the smallest allocation size
            if (v < m_sectorSize)
            {
                return m_sectorSize;
            }
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;

        }

        public void Flush()
        {
            lock(m_lock)
            {
                foreach(var writer in segmentWriters)
                {
                    writer.Value.Flush();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    lock (m_lock)
                    {
                        foreach(var segment in segmentWriters)
                        {
                            segment.Value.Dispose();
                        }
                        segmentWriters.Clear();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Malloc()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
