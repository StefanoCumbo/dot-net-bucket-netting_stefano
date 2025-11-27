using FundApps.Engine.Ingestion;
using FundApps.Engine.Persistence;
using FundApps.Engine.Rules;

var xml = File.ReadAllText("sample-portfolio.xml");
var portfolioResult = PortfolioXmlIngestor.FromXml(xml);
if (!portfolioResult.Success)
{
    Console.WriteLine("Failed to parse portfolio:");
    foreach (var error in portfolioResult.Errors)
    {
        Console.WriteLine($"- {error}");
    }
    return;
}

var rule = new RuleProcessor();
var results = rule.MaxContinentalOwnershipMustNotBreach15Percent(portfolioResult.Portfolio);

var violation = rule.FuturesMustNotMatureWithin7Days(portfolioResult.Portfolio);

var store = new InMemoryResultStore();
store.Save(results);

foreach (var result in results)
{
    Console.WriteLine($"{result.Continent} - Breached: {result.IsBreached} - Message: {result.Message}");
    
   
}

foreach (var result in violation)
{
    Console.WriteLine($"{result.Continent} - Breached: {result.IsBreached} - Message: {result.Message}");
    
   
}



