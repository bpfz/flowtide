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

namespace FlowtideDotNet.Core
{
    /// <summary>
    /// Represents a batch of stream events.
    /// A schema describes the name of the columns that is used for all events in this batch.
    /// The schema does not contain the data type in the column, since that can differ between events.
    /// </summary>
    public class StreamEventBatch
    {
        public Schema Schema { get; }

        public IReadOnlyList<StreamEvent> Events { get; }

        public StreamEventBatch(Schema schema, IReadOnlyList<StreamEvent> events)
        {
            Schema = schema;
            Events = events;
        }
    }
}
