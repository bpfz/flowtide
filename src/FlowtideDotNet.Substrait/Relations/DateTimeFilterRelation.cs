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

namespace FlowtideDotNet.Substrait.Relations
{
    /// <summary>
    /// An extension relation that is used to do a filter based on the current time.
    /// This is a seperate relation to enable an implementation that 
    /// </summary>
    public class DateTimeFilterRelation : Relation
    {
        public Relation Input { get; set; }

        public override int OutputLength => Input.OutputLength;

        public override TReturn Accept<TReturn, TState>(RelationVisitor<TReturn, TState> visitor, TState state)
        {
            throw new NotImplementedException();
        }
    }
}
