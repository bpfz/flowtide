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

namespace FlowtideDotNet.Storage.Tree.Internal
{
    internal class BPlusTreeIterator<K, V> : IBPlusTreeIterator<K, V>
    {
        internal class Enumerator : IAsyncEnumerator<IBPlusTreePageIterator<K, V>>
        {
            private readonly BPlusTree<K, V> tree;
            private LeafNode<K, V>? leafNode;
            private int index;
            private bool started;
            //private ValueTask<IBPlusTreeNode?>? getNextNodeTask;

            public Enumerator(in BPlusTree<K, V> tree)
            {
                this.tree = tree;
            }

            public void Reset(LeafNode<K, V> leafNode, int index)
            {
                this.leafNode = leafNode;
                this.index = index;
                //getNextNodeTask = null;
                this.started = false;
            }

            public IBPlusTreePageIterator<K, V> Current => new BPlusTreePageIterator<K, V>(leafNode!, index, tree);

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                if (leafNode == null)
                {
                    return ValueTask.FromResult(false);
                }
                if (!started)
                {
                    started = true;
                    return ValueTask.FromResult(true);
                }
                //if (getNextNodeTask.HasValue)
                //{
                //    if (!getNextNodeTask.Value.IsCompleted)
                //    {
                //        return MoveNextAsync_Slow(getNextNodeTask.Value);
                //    }
                //    leafNode = (getNextNodeTask.Value.Result as LeafNode<K, V>)!;
                //    index = 0;
                //    if (leafNode.next > 0)
                //    {
                //        getNextNodeTask = tree.m_stateClient.GetValue(leafNode.next, "MoveNextAsync1");
                //    }
                //    else
                //    {
                //        getNextNodeTask = default;
                //    }
                //    return ValueTask.FromResult(true);
                //}
                if (leafNode.next == 0)
                {
                    return ValueTask.FromResult(false);
                }
                var getNextPageTask = tree.m_stateClient.GetValue(leafNode.next, "MoveNextAsync2");

                if (!getNextPageTask.IsCompleted)
                {
                    return MoveNextAsync_Slow(getNextPageTask);
                }
                leafNode = (getNextPageTask.Result as LeafNode<K, V>)!;
                index = 0;
                //if (leafNode.next > 0)
                //{
                //    getNextNodeTask = tree.m_stateClient.GetValue(leafNode.next, "MoveNextAsync3");
                //}
                return ValueTask.FromResult(true);
            }

            private async ValueTask<bool> MoveNextAsync_Slow(ValueTask<IBPlusTreeNode?> getPageTask)
            {
                var page = await getPageTask;
                leafNode = (page as LeafNode<K, V>)!;
                index = 0;
                //if (leafNode.next > 0)
                //{
                //    getNextNodeTask = tree.m_stateClient.GetValue(leafNode.next, "MoveNextAsync_Slow");
                //}
                //else
                //{
                //    getNextNodeTask = default;
                //}
                return true;
            }
        }

        private LeafNode<K, V> leafNode;
        private readonly BPlusTree<K, V> tree;
        private int index;
        private readonly Enumerator enumerator;

        public BPlusTreeIterator(BPlusTree<K, V> tree)
        {
            this.tree = tree;
            enumerator = new Enumerator(tree);
        }

        public IAsyncEnumerator<IBPlusTreePageIterator<K, V>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return enumerator;
        }

        public ValueTask Seek(in K key, in IComparer<K>? searchComparer = null)
        {
            var comparer = searchComparer == null ? tree.m_keyComparer : searchComparer;
            var searchTask = tree.SearchRoot(key, comparer);
            if (!searchTask.IsCompleted)
            {
                return Seek_Slow(searchTask, key, comparer);
            }
            leafNode = searchTask.Result;
            AfterSeek(key, comparer);
            return ValueTask.CompletedTask;
        }

        private async ValueTask Seek_Slow(ValueTask<LeafNode<K, V>> task, K key, IComparer<K> searchComparer)
        {
            leafNode = await task;
            AfterSeek(key, searchComparer);
        }

        private void AfterSeek(in K key, IComparer<K> searchComparer)
        {
            var i = leafNode.keys.BinarySearch(key, searchComparer);
            if (i < 0)
            {
                i = ~i;
            }
            index = i;
            if (index >= leafNode.keys.Count && leafNode.next == 0)
            {
                leafNode = null;
            }
            enumerator.Reset(leafNode, index);
        }

        public async ValueTask SeekFirst()
        {
            leafNode = await tree.LeftLeaf();
            index = 0;
            enumerator.Reset(leafNode, index);
        }
    }
}
