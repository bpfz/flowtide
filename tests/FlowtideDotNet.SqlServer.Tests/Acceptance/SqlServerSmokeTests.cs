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

using FlowtideDotNet.Core.Engine;
using FlowtideDotNet.Core.Tests.SmokeTests;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace FlowtideDotNet.SqlServer.Tests.Acceptance
{
    public class SqlServerSmokeTests : QuerySmokeTestBase, IClassFixture<SqlServerFixture>
    {
        private readonly SqlServerFixture sqlServerFixture;
        public SqlServerSmokeTests(SqlServerFixture sqlServerFixture)
        {
            this.sqlServerFixture = sqlServerFixture;
        }

        public override async Task AddLineItems(IEnumerable<LineItem> lineItems)
        {
            await sqlServerFixture.dbContext.BulkInsertAsync(lineItems);
        }

        public override async Task AddOrders(IEnumerable<Order> orders)
        {
            await sqlServerFixture.dbContext.BulkInsertAsync(orders);
        }

        public override void AddReadResolvers(ReadWriteFactory readWriteFactory)
        {
            readWriteFactory.AddSqlServerSource(".*", () => sqlServerFixture.ConnectionString, (rel) =>
            {
                var name = rel.NamedTable.Names[0];
                rel.NamedTable.Names = new List<string>() { "tpch", "dbo", name };
            });
        }

        public override async Task AddShipmodes(IEnumerable<Shipmode> shipmodes)
        {
            await sqlServerFixture.dbContext.BulkInsertAsync(shipmodes);
        }

        public override async Task ClearAllTables()
        {
            await sqlServerFixture.dbContext.LineItems.ExecuteDeleteAsync();
            await sqlServerFixture.dbContext.Orders.ExecuteDeleteAsync();
            await sqlServerFixture.dbContext.Shipmodes.ExecuteDeleteAsync();
        }

        public override async Task UpdateShipmodes(IEnumerable<Shipmode> shipmode)
        {
            await sqlServerFixture.dbContext.BulkUpdateAsync(shipmode);
        }
    }
}
