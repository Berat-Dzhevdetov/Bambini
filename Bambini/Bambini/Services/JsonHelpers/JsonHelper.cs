namespace Bambini.Services.JsonHelpers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Text;

    internal static class JsonHelper// : IJsonHelper
    {
        public static string GetField(string json, string field)
        {
            var data = JObject.Parse(json);

            if (data == null)
                return null;

            var property = data.Property(field).Value.ToString();

            if (property == null) return null;

            return property.ToString();
        }
    }
}
