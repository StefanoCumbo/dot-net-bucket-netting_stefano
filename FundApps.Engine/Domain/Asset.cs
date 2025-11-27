namespace FundApps.Engine.Domain
{
    public abstract class Asset
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Class { get; set; }
        public string CountryCode { get; set; }
        public int Quantity { get; set; }
        public int TotalSharesOutstanding { get; set; }
        public DateTime? MaturityDate { get; set; }
    }

    public class Equity : Asset
    {
        public Equity()
        {
            Class = "Equity";
        }
    }

    public class Future : Asset
    {
        public Future()
        {
            Class = "Future";
        }
    }
}
