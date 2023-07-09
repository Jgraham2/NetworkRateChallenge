using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CodingChallenge.Utils
{
    public interface IFileUtils
    {
        JObject ConvertDotFileToJson(string dotFileData);
    }

    public class FileUtils : IFileUtils
    {
        public JObject ConvertDotFileToJson(string dotFileData)
        {
            var typesRegex = new Regex(@"(\w+)\s+\[type=(\w+)\];");
            var connectionsRegex = new Regex(@"(\w+)\s+--\s+(\w+)\s+\[length=(\d+),\s+material=(\w+)\];");

            var typesMatches = typesRegex.Matches(dotFileData);
            var types = new JObject();
            foreach (Match match in typesMatches)
            {
                var nodeName = match.Groups[1].Value;
                var nodeType = match.Groups[2].Value;
                types.Add(nodeName, new JObject(new JProperty("type", nodeType)));
            }

            var connectionsMatches = connectionsRegex.Matches(dotFileData);
            var connections = new JObject();
            foreach (Match match in connectionsMatches)
            {
                var node1 = match.Groups[1].Value;
                var node2 = match.Groups[2].Value;
                var length = match.Groups[3].Value;
                var material = match.Groups[4].Value;

                var connectionName = $"{node1} -- {node2}";
                connections.Add(connectionName, new JObject(new JProperty("length", length), new JProperty("material", material)));
            }

            var jsonGraph = new JObject(new JProperty("types", types), new JProperty("connections", connections));
            return jsonGraph;

        }

        private static JObject ParseProperties(string[] properties)
        {
            var nodeJson = new JObject();

            foreach (var property in properties)
            {
                var keyValue = property.Split('=');

                if (keyValue.Length == 2)
                {
                    var propName = keyValue[0].Trim();
                    var propValue = keyValue[1].Trim().Trim('"');

                    nodeJson[propName] = propValue;
                }
            }

            return nodeJson;
        }

    }
}
