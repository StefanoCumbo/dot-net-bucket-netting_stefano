namespace FundApps.Engine.Domain
{
    public class RuleResult
    {
        public bool IsBreached { get; init; }
        public string Continent { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
