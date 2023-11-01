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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowtideDotNet.Substrait.Sql.Internal
{
    internal class CTEContainer
    {
        public CTEContainer(string alias, EmitData emitData, int outputLength)
        {
            Alias = alias;
            EmitData = emitData;
            UsageCounter = 0;
            OutputLength = outputLength;
        }

        public string Alias { get; }
        public EmitData EmitData { get; }

        public int OutputLength { get; }

        /// <summary>
        /// Counter that increments if the CTE is used inside its own query, this will mark it as recursive.
        /// </summary>
        public int UsageCounter { get; set; }
    }
}
