using Xunit.Abstractions;

namespace FlowtideDotNet.AcceptanceTests
{
    public class SelectTests : FlowtideAcceptanceBase
    {
        public SelectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task SelectTwoColumnsWithId()
        {
            GenerateData();
            await StartStream("INSERT INTO output SELECT userkey, firstName FROM users");
            await WaitForUpdate();
            AssertCurrentDataEqual(Users.Select(x => new { x.UserKey, x.FirstName }));
        }

        [Fact]
        public async Task SelectOneColumnsWithoutId()
        {
            GenerateData();
            await StartStream("INSERT INTO output SELECT firstName FROM users");
            await WaitForUpdate();
            AssertCurrentDataEqual(Users.Select(x => new { x.FirstName }));
        }

        [Fact]
        public async Task SelectWithCase()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    CASE WHEN Gender = 1 THEN firstName 
                    ELSE lastName 
                    END AS name
                FROM users");
            await WaitForUpdate();
            AssertCurrentDataEqual(Users.Select(x => new { Name = x.Gender == Entities.Gender.Female ? x.FirstName : x.LastName }));
        }

        [Fact]
        public async Task SelectWithCaseNoElse()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    CASE WHEN Gender = 1 THEN firstName
                    END AS name
                FROM users");
            await WaitForUpdate();
            AssertCurrentDataEqual(Users.Select(x => new { Name = x.Gender == Entities.Gender.Female ? x.FirstName : null }));
        }

        [Fact]
        public async Task SelectWithConcat()
        {
            GenerateData();
            await StartStream(@"
                INSERT INTO output 
                SELECT 
                    firstName || ' ' || lastName AS name
                FROM users");
            await WaitForUpdate();
            AssertCurrentDataEqual(Users.Select(x => new { Name = x.FirstName + " " + x.LastName }));
        }
    }
}