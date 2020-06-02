using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Runtime;

namespace Zyborg.VMBot.Util
{
    public class ConstantClassConverter : JsonConverter<ConstantClass>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(ConstantClass).IsAssignableFrom(typeToConvert);

        public override ConstantClass Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return FindValue(typeToConvert, reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, ConstantClass value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

        private static readonly Type[] StringParams = new[] { typeof(string) };

        public static ConstantClass FindValue(Type typeToConvert, string value)
        {
            // First try to find a public method for the specific type
            // which is the convention for ConstantClass-derived types
            var find = typeToConvert.GetMethod(nameof(FindValue),
                BindingFlags.Public | BindingFlags.Static,
                null, StringParams, null);
            
            // If we can't find that, then default to the
            // protected generic form of the base class
            if (find == null)
            {
                find = typeToConvert.GetMethod(nameof(FindValue), 1,
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null, StringParams, null); 
                if (find == null)
                    throw new Exception("could not resolve lookup method");
                
                // Get a concrete method for the specific target type
                find = find.MakeGenericMethod(new[] { typeToConvert });
            }

            return (ConstantClass)find.Invoke(null, new[] { value });
        }
    }
}