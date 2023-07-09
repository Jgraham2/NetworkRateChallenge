using Newtonsoft.Json.Linq;

namespace CodingChallenge.Services.Rates
{

    public class RateCalculator
    {
        public static JObject ParseRates(JObject rateCardData)
        {
            return rateCardData;
        }

        public static Dictionary<string, decimal> CalculateRateCardTotals(JObject rateCardData, JObject jsonGraph)
        {
            var rateCardTotals = new Dictionary<string, decimal>();

            foreach (var rateCard in rateCardData)
            {
                var rateCardName = rateCard.Key;
                decimal totalCost = 0;

                foreach (var connection in jsonGraph["connections"]!.Children<JProperty>())
                {
                    var connectionName = connection.Name;

                    if (connectionName.Contains("--"))
                    {
                        var nodes = connectionName.Split(new[] { " -- " }, StringSplitOptions.RemoveEmptyEntries);
                        var node1 = nodes[0];
                        var node2 = nodes[1];

                        if (jsonGraph["types"]?[node1] != null && jsonGraph["types"]?[node2] != null)
                        {
                            var node1Type = jsonGraph["types"]?[node1]?["type"]?.ToString();
                            var node2Type = jsonGraph["types"]?[node2]?["type"]?.ToString();

                            if (rateCardData[rateCardName]?["items"] is JArray rateCardItems)
                            {
                                if (node1Type != null)
                                {
                                    var node1Cost = GetItemCost(rateCardItems, node1Type);
                                    if (node2Type != null)
                                    {
                                        var node2Cost = GetItemCost(rateCardItems, node2Type);

                                        var length = decimal.Parse(connection.Value["length"]!.ToString());
                                        var costPerMeter = (string)connection.Value["material"]! == "verge"
                                            ? GetItemCost(rateCardItems, "Trench/m (verge)")
                                            : GetItemCost(rateCardItems, "Trench/m (road)");

                                        var connectionCost = (node1Cost + node2Cost) * length + costPerMeter * length;

                                        totalCost += connectionCost;
                                    }
                                }
                            }
                        }
                    }
                }

                rateCardTotals[rateCardName] = totalCost;
            }

            return rateCardTotals;
        }

        private static decimal GetItemCost(JArray rateCardItems, string itemName)
        {
            foreach (var item in rateCardItems)
            {
                var name = item["name"]?.ToString();
                if (name != null && name.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                {
                    var costString = item["cost"]?.ToString();
                    if (decimal.TryParse(costString, out var cost))
                    {
                        return cost;
                    }
                }
            }

            return 0;
        }

    }

}
