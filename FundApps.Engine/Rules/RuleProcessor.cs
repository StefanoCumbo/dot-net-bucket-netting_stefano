using FundApps.Engine.Domain;

namespace FundApps.Engine.Rules
{
    public class RuleProcessor
    {
        private static readonly Dictionary<string, string> CountryToContinent = new(StringComparer.OrdinalIgnoreCase)
        {
            { "US", "North America" },
            { "CA", "North America" },
            { "GB", "Europe" },
            { "DE", "Europe" },
            { "FR", "Europe" },
            { "JP", "Asia" },
            // Add more as needed
        };
        
        // Institutions must not own more than 15% of the assets registered in each continent in their portfolio
        public IReadOnlyList<RuleResult> MaxContinentalOwnershipMustNotBreach15Percent(Portfolio portfolio)
        {
            var continentTotals = new Dictionary<string, (decimal Owned, decimal Total)>();
            var companyAlreadySeen = new HashSet<string>();
            foreach (var asset in portfolio.Assets)
            {
                
                // skip unknown countries
                // TODO we should probably log this
                if (!CountryToContinent.TryGetValue(asset.CountryCode, out var continent))
                {
                    continue;
                }

                if (!continentTotals.TryGetValue(continent, out var value))
                {
                    value = (0, 0);
                    continentTotals[continent] = value;
                }
                var (owned, total) = value;
                
                owned += asset.Quantity;
                if (companyAlreadySeen.Add(asset.CompanyName))
                {
                    total += asset.TotalSharesOutstanding;
                }
                
                continentTotals[continent] = (owned, total);
            }
            
            var results = new List<RuleResult>();
            foreach (var (continent, (continentOwned, continentTotal)) in continentTotals)
            {
                var percent = continentTotal == 0 ? 0 : continentOwned / continentTotal * 100;
                if (percent > 15)
                {
                    results.Add(new RuleResult
                    {
                        IsBreached = true,
                        Continent = continent,
                        Message = $"Ownership ({percent}%) exceeds 15%"
                    });
                }
                else
                {
                    results.Add(new RuleResult
                    {
                        IsBreached = false,
                        Continent = continent,
                        Message = $"Ownership ({percent}%) is within allowed range"
                    });
                }
            }

            return results;
        }
        
        public List<RuleResult> FuturesMustNotMatureWithin7Days(Portfolio portfolio)
        {
            var violations = new List<RuleResult>();
            var sevenWorkingDaysFromNow = CalculateWorkingDaysAhead(DateTime.Today, 7);
    
            foreach (var asset in portfolio.Assets)
            {
                
                
                if (asset.MaturityDate == null)
                {
                    violations.Add(new RuleResult
                    {
                        IsBreached = true,
                        Continent = asset.CompanyName,
                        Message = $"Asset {asset.CompanyName} has no maturity date"
                    });
                    continue;
                }
                
                if (asset.MaturityDate.Value < DateTime.Today)
                {
                    // TODO: Log warning - future with past maturity date
                    // This suggests stale data or expired contracts still in portfolio
                    violations.Add(new RuleResult()
                    {
                        IsBreached = true,
                        Continent = asset.CompanyName,
                        Message = $"asset {asset.CompanyName} is exipred"
                    });
                    continue;
                }
            
        
                // Check if the future has a maturity date
                if (asset.MaturityDate == null)
                {
                    // TODO: Log warning - future without maturity date
                    violations.Add((new RuleResult()
                    {
                        IsBreached = true,
                        Continent = asset.CompanyName,
                        Message = $"asset {asset.CompanyName} has no maturity date"
                    }));
                    
                    continue;
                }
        
                // If maturity date is within the next 7 working days, it's a violation
                if (asset.MaturityDate.Value <= sevenWorkingDaysFromNow)
                {
                    violations.Add(new RuleResult()
                    {
                        IsBreached = true,
                        Continent = asset.CompanyName,
                        Message = $" asset{asset.CompanyName} is maturing in the next 7 days"
                    });
                }
                
                
                else
                {
                    // Rule not breached - future matures after 7 working days
                    violations.Add(new RuleResult()
                    {
                        IsBreached = false,
                        Continent = asset.CompanyName,
                        Message = $"Asset {asset.CompanyName} maturity date is acceptable (matures on {asset.MaturityDate.Value:yyyy-MM-dd})"
                    });
                }
            }
    
            return violations;
        }


      
        
        private DateTime CalculateWorkingDaysAhead(DateTime startDate, int workingDays)
        {
            var currentDate = startDate;
            var daysAdded = 0;
    
            while (daysAdded < workingDays)
            {
                currentDate = currentDate.AddDays(1);
        
                // Skip weekends
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && 
                    currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysAdded++;
                }
            }
    
            return currentDate;
        }
        
        
        
        
        
        
        
        
    }
}
