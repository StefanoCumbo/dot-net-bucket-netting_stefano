namespace FundApps.Engine.Domain
{
    public class Portfolio
    {
        public DateTime Date { get; init; }
        public IReadOnlyList<Asset> Assets { get; init; } = [];
    }
}
