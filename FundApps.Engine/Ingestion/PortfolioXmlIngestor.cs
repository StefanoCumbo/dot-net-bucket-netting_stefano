using System.Globalization;
using System.Xml.Linq;
using FundApps.Engine.Domain;

namespace FundApps.Engine.Ingestion
{
    public static class PortfolioXmlIngestor
    {
        public static PortfolioParseResult FromXml(string xml)
        {
            var errors = new List<string>();
            Portfolio? portfolio = null;
            try
            {
                var doc = XDocument.Parse(xml);
                var portfolioElem = doc.Element("Portfolio");
                if (portfolioElem == null)
                {
                    errors.Add("Missing Portfolio element.");
                    return PortfolioParseResult.Errored(errors);
                }

                DateTime date = default;
                var dateElem = portfolioElem.Element("Date");
                if (dateElem == null || string.IsNullOrWhiteSpace(dateElem.Value))
                {
                    errors.Add("Missing or empty Date element.");
                }
                else if (!DateTime.TryParse(dateElem.Value, out date))
                {
                    errors.Add($"Invalid Date value: '{dateElem.Value}'");
                }

                var assetsElem = portfolioElem.Element("Assets");
                if (assetsElem == null)
                {
                    errors.Add("Missing Assets element.");
                    return PortfolioParseResult.Errored(errors);
                }

                var assets = new List<Asset>();
                foreach (var assetElem in assetsElem.Elements("Asset"))
                {
                    var classType = assetElem.Element("Class")?.Value;
                    
                    Asset asset;
                    switch (classType)
                    {
                        case "Future":
                            asset = new Future();
                            break;
                        case "Equity":
                            asset = new Equity();
                            break;
                        default:
                            errors.Add("Asset is missing Class or has an invalid Class.");
                            continue; // Skip this asset if class is invalid
                    }

                    var id = assetElem.Element("Id")?.Value;
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        errors.Add("Asset is missing Id.");
                    }
                    asset.Id = id ?? string.Empty;

                    var companyName = assetElem.Element("CompanyName")?.Value;
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        errors.Add("Asset is missing company name.");
                    }
                    asset.CompanyName = companyName ?? string.Empty;
                    
                    var countryCode = assetElem.Element("CountryCode")?.Value;
                    if (string.IsNullOrWhiteSpace(countryCode))
                    {
                        errors.Add($"Asset {id} is missing CountryCode.");
                    }
                    asset.CountryCode = countryCode ?? string.Empty;

                    var quantityStr = assetElem.Element("Quantity")?.Value;
                    if (!int.TryParse(quantityStr, out var quantity))
                    {
                        errors.Add($"Asset {id} has invalid Quantity: '{quantityStr}'");
                    }
                    asset.Quantity = quantity;

                    var tsoStr = assetElem.Element("TotalSharesOutstanding")?.Value;
                    if (!int.TryParse(tsoStr, out var tso))
                    {
                        errors.Add($"Asset {id} has invalid TotalSharesOutstanding: '{tsoStr}'");
                    }
                    asset.TotalSharesOutstanding = tso;

                    var maturity = assetElem.Element("MaturityDate")?.Value;
                    if (!string.IsNullOrEmpty(maturity))
                    {
                        if (DateTime.TryParse(maturity, CultureInfo.InvariantCulture, DateTimeStyles.None, out var maturityDate))
                        {
                            asset.MaturityDate = maturityDate;
                        }
                        else
                        {
                            errors.Add($"Asset {id} has invalid MaturityDate: '{maturity}'");
                        }
                    }

                    assets.Add(asset);
                }

                if (errors.Count == 0)
                {
                    portfolio = new Portfolio { Date = date, Assets = assets };
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Exception during parsing: {ex.Message}");
            }
            
            return new PortfolioParseResult
            {
                Success = errors.Count == 0,
                Portfolio = portfolio,
                Errors = errors
            };
        }
    }
}
