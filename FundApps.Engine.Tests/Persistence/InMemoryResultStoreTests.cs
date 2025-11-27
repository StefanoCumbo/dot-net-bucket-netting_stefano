using FundApps.Engine.Domain;
using FundApps.Engine.Persistence;

namespace FundApps.Engine.Tests.Persistence
{
    public class InMemoryResultStoreTests
    {
        [Fact]
        public void CanSaveAndRetrieveResults()
        {
            var store = new InMemoryResultStore();
            var result = new RuleResult { Continent = "Europe", IsBreached = false, Message = "No breach" };
            
            store.Save([result]);
            
            var all = store.GetAll();
            var storedResult = Assert.Single(all);
            Assert.Equal("No breach", storedResult.Message);
        }
    }
}
