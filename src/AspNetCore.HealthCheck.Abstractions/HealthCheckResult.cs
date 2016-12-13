using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AspNetCore.HealthCheck
{
    public class HealthCheckResult
    {
        public string Name { get; set; }

        public HealthStatus Status { get; set; }

        public long Elapsed { get; set; }

        public string Message { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> Properties { get; set; } = new Dictionary<string, JToken>();

        public long Issued { get; set; }

        public bool Critical { get; set; }

        public IList<string> Tags { get; set; }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }

        private static class LogSerializer
        {
            static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            static LogSerializer()
            {
                jsonSettings.Converters.Add(new StringEnumConverter());
            }

            public static string Serialize(object logObject)
            {
                return JsonConvert.SerializeObject(logObject, jsonSettings);
            }
        }
    }
}