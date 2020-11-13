using System;
using Newtonsoft.Json.Linq;

namespace Zyborg.AWS.Lambda
{
    public static class NewtonsoftJsonExtensions
    {
        public static bool HasJPath(this JToken jt, string path,
            Func<JToken, bool> childTokenTester = null)
        {
            var jtChild = jt.SelectToken(path);
            if (jtChild != null)
            {
                if (childTokenTester == null || childTokenTester(jtChild))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasJValue(this JToken jt, string path, string matchValue = null)
        {
            return HasJPath(jt, path, jtChild => jtChild is JValue jv
                && (matchValue == null || string.Equals(matchValue, jv.Value?.ToString())));
        }
    }
}