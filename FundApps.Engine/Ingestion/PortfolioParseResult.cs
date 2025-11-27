using System.Diagnostics.CodeAnalysis;
using FundApps.Engine.Domain;

namespace FundApps.Engine.Ingestion
{
    public class PortfolioParseResult
    {
        [MemberNotNullWhen(true, nameof(Portfolio))]
        public bool Success { get; init; }
        public Portfolio? Portfolio { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = [];
        
        public static PortfolioParseResult Successful(Portfolio portfolio)
        {
            return new PortfolioParseResult
            {
                Success = true,
                Portfolio = portfolio
            };
        }
        
        public static PortfolioParseResult Errored(IReadOnlyList<string> errors)
        {
            return new PortfolioParseResult
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
