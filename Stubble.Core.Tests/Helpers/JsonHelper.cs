using System.Linq;
using Newtonsoft.Json.Linq;

namespace Stubble.Core.Tests.Helpers
{
    public static class JsonHelper
    {
        internal static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                        .ToDictionary(prop => prop.Name,
                            prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}