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

using FlowtideDotNet.Base.Vertices.Ingress;
using FlowtideDotNet.Core;
using FlowtideDotNet.Core.Operators.Read;
using FlowtideDotNet.Storage.StateManager;
using FlowtideDotNet.Substrait.Relations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FlowtideDotNet.AcceptanceTests.Internal
{
    internal class MockDataSourceState
    {

    }
    internal class MockDataSourceOperator : ReadBaseOperator<MockDataSourceState>
    {
        private readonly ReadRelation readRelation;
        private readonly MockDatabase mockDatabase;
        private HashSet<string> _watermarkNames;
        private MockTable _table;
        private int _lastestOffset;

        public MockDataSourceOperator(ReadRelation readRelation, MockDatabase mockDatabase, DataflowBlockOptions options) : base(options)
        {
            this.readRelation = readRelation;
            this.mockDatabase = mockDatabase;

            _table = mockDatabase.GetTable(readRelation.NamedTable.DotSeperated);

            _watermarkNames = new HashSet<string>() { readRelation.NamedTable.DotSeperated };
        }

        public override string DisplayName => "Mock data source";

        public override Task DeleteAsync()
        {
            return Task.CompletedTask;
        }

        public override Task OnTrigger(string triggerName, object? state)
        {
            return Task.CompletedTask;
        }

        protected override Task<IReadOnlySet<string>> GetWatermarkNames()
        {
            return Task.FromResult<IReadOnlySet<string>>(_watermarkNames);
        }

        protected override Task InitializeOrRestore(long restoreTime, MockDataSourceState? state, IStateManagerClient stateManagerClient)
        {
            return Task.CompletedTask;
        }

        protected override Task<MockDataSourceState> OnCheckpoint(long checkpointTime)
        {
            return Task.FromResult(new MockDataSourceState());
        }

        protected override async Task SendInitial(IngressOutput<StreamEventBatch> output)
        {
            await output.EnterCheckpointLock();
            var (operations, fetchedOffset) = _table.GetOperations(_lastestOffset);

            List<StreamEvent> o = new List<StreamEvent>();
            foreach(var operation in operations)
            {
                o.Add(MockTable.ToStreamEvent(operation, readRelation.BaseSchema.Names));
                //o.Add(new StreamEvent(1, 0, operation.Vector));

                if (o.Count > 100)
                {
                    await output.SendAsync(new StreamEventBatch(null, o));
                    o = new List<StreamEvent>();
                }
            }

            if (o.Count > 0)
            {
                await output.SendAsync(new StreamEventBatch(null, o));
            }
            await output.SendWatermark(new Base.Watermark(readRelation.NamedTable.DotSeperated, fetchedOffset));
            output.ExitCheckpointLock();
            this.ScheduleCheckpoint(TimeSpan.FromSeconds(1));
        }
    }
}
