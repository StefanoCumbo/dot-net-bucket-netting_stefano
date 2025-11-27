using FundApps.Engine.Domain;
using FundApps.Engine.Ingestion;

namespace FundApps.Engine.Tests.Ingestion
{
    public class PortfolioXmlIngestorTests
    {
        [Fact]
        public void CanParseSamplePortfolioXml()
        {
            const string xml = "<Portfolio><Date>2025-06-25</Date><Assets><Asset><Id>EQ1</Id><CompanyName>McDonalds</CompanyName><Class>Equity</Class><CountryCode>US</CountryCode><Quantity>100</Quantity><TotalSharesOutstanding>1000</TotalSharesOutstanding></Asset></Assets></Portfolio>";
            var portfolioResult = PortfolioXmlIngestor.FromXml(xml);

            Assert.True(portfolioResult.Success);
            var portfolio = portfolioResult.Portfolio;
            
            var asset = Assert.Single(portfolio.Assets);
            var equity = Assert.IsType<Equity>(asset);
            Assert.Equal("McDonalds", equity.CompanyName);
            Assert.Equal("US", equity.CountryCode);
        }
        
        [Fact]
        public void ReturnsErrorForInvalidXml()
        {
            const string xml = "<Portfolio><Date>InvalidDate</Date></Portfolio>";
            var portfolioResult = PortfolioXmlIngestor.FromXml(xml);

            Assert.False(portfolioResult.Success);
            Assert.Contains("Invalid Date value: 'InvalidDate'", portfolioResult.Errors);
        }
    }
}
