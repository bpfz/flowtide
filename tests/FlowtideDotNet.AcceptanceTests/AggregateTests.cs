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
using Xunit.Abstractions;

namespace FlowtideDotNet.AcceptanceTests
{
    public class AggregateTests : FlowtideAcceptanceBase
    {
        public AggregateTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task AggregateCount()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    count(*)
                FROM orders o");
            await WaitForUpdate();
            AssertCurrentDataEqual(new[] { new { Count = Orders.Count() } });
        }

        [Fact]
        public async Task AggregateCountWithGroup()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    userkey, count(*)
                FROM orders
                GROUP BY userkey");
            await WaitForUpdate();

            AssertCurrentDataEqual(Orders.GroupBy(x => x.UserKey).Select(x => new { UserKey = x.Key, Count = x.Count() }));
        }

        [Fact]
        public async Task AggregateOnJoinedData()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    u.firstName, count(*)
                FROM orders o
                INNER JOIN users u
                ON o.userkey = u.userkey
                GROUP BY u.firstName");
            await WaitForUpdate();

            AssertCurrentDataEqual(Orders.Join(Users, x => x.UserKey, x => x.UserKey, (l, r) => new { l.OrderKey, r.FirstName }).GroupBy(x => x.FirstName).Select(x => new { FirstName = x.Key, Count = x.Count() }));
        }

        [Fact]
        public async Task MultipleAggregates()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    userkey, sum(orderkey), count(*)
                FROM orders
                GROUP BY userkey
                ");
            await WaitForUpdate();

            AssertCurrentDataEqual(Orders
                .GroupBy(x => x.UserKey)
                .Select(x => new { Userkey = x.Key, Sum = (double)x.Sum(y => y.OrderKey), Count = x.Count() }));
        }

        [Fact]
        public async Task HavingSameAggregate()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    userkey, count(*)
                FROM orders
                GROUP BY userkey
                HAVING count(*) > 1
                ");
            await WaitForUpdate();

            AssertCurrentDataEqual(Orders.GroupBy(x => x.UserKey).Select(x => new { FirstName = x.Key, Count = x.Count() }).Where(x => x.Count > 1));
        }

        [Fact]
        public async Task HavingDifferentAggregate()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    userkey, sum(orderkey)
                FROM orders
                GROUP BY userkey
                HAVING count(*) > 1
                ");
            await WaitForUpdate();

            AssertCurrentDataEqual(Orders
                .GroupBy(x => x.UserKey)
                .Select(x => new { Userkey = x.Key, Count = x.Count(), Sum = (double)x.Sum(y => y.OrderKey) })
                .Where(x => x.Count > 1)
                .Select(x => new {x.Userkey, x.Sum}));
        }

        [Fact]
        public async Task AggregateWithStateCrash()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    userkey, min(orderkey)
                FROM orders
                GROUP BY userkey
                ");
            await WaitForUpdate();

            await Crash();

            GenerateData(1000);

            await WaitForUpdate();



            AssertCurrentDataEqual(Orders.GroupBy(x => x.UserKey).Select(x => new { UserKey = x.Key, MinVal = x.Min(y => y.OrderKey) }));
        }
    }
}
