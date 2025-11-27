using FundApps.Engine.Domain;
using FundApps.Engine.Rules;

namespace FundApps.Engine.Tests.Rules
{
    public class RuleProcessorTests
    {
        [Fact]
        public void DetectsBreach_WhenOwnershipExceeds15Percent()
        {
            var portfolio = new Portfolio
            {
                Date = DateTime.Now,
                Assets =
                [
                    new Equity { Id = "Apple", CompanyName = "Apple", CountryCode = "US", Quantity = 30000, TotalSharesOutstanding = 200000 },
                    new Future { Id = "Apple", CompanyName = "Apple", CountryCode = "US", Quantity = 30000, TotalSharesOutstanding = 200000 },
                    new Equity { Id = "Microsoft", CompanyName = "Microsoft", CountryCode = "US", Quantity = 30000, TotalSharesOutstanding = 300000 }
                ]
            };
            var rule = new RuleProcessor();
            
            var results = rule.MaxContinentalOwnershipMustNotBreach15Percent(portfolio);
            
            var result = Assert.Single(results);
            Assert.True(result.IsBreached);
            Assert.Equal("Ownership (18.00%) exceeds 15%", result.Message);
        }

        [Fact]
        public void NoBreach_WhenOwnershipBelow15Percent()
        {
            var portfolio = new Portfolio
            {
                Date = DateTime.Now,
                Assets =
                [
                    new Equity { Id = "Apple", CompanyName = "Apple", CountryCode = "US", Quantity = 3000, TotalSharesOutstanding = 200000 },
                    new Equity { Id = "Microsoft", CompanyName = "Microsoft", CountryCode = "US", Quantity = 30000, TotalSharesOutstanding = 300000 }
                ]
            };
            var rule = new RuleProcessor();
            
            var results = rule.MaxContinentalOwnershipMustNotBreach15Percent(portfolio);

            var result = Assert.Single(results);
            Assert.False(result.IsBreached);
        }
    }
}
