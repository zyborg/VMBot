using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zyborg.GuacBot.GuacAPI
{
    public class EnumCustomConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(EnumCustomConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);
            
            return converter;
        }
    }

    public class EnumCustomConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        private static readonly TypeCode EnumTypeCode = Type.GetTypeCode(typeof(T));
        // Odd type codes are conveniently signed types (for enum backing types).
        private static readonly string? NegativeSign = ((int)EnumTypeCode % 2) == 0
            ? null : NumberFormatInfo.CurrentInfo.NegativeSign;
        private const string ValueSeparator = ", ";

        private readonly ConcurrentDictionary<ulong, JsonEncodedText> _nameCache;

        // This is used to prevent flooding the cache due to exponential bitwise combinations of flags.
        // Since multiple threads can add to the cache, a few more values might be added.
        private const int NameCacheSizeSoftLimit = 64;

        public EnumCustomConverter(JsonSerializerOptions serializerOptions)
        {
            _nameCache = new ConcurrentDictionary<ulong, JsonEncodedText>();
        }

        // public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        // {
        //     throw new NotImplementedException();
        // }

        // public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        // {
        //     throw new NotImplementedException();
        // }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonTokenType token = reader.TokenType;

            if (token == JsonTokenType.String)
            {
                return ReadWithQuotes(ref reader);
            }
            throw new JsonException("unexpected JSON token for enum; expected string");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            ulong key = ConvertToUInt64(value);

            if (_nameCache.TryGetValue(key, out JsonEncodedText formatted))
            {
                writer.WriteStringValue(formatted);
                return;
            }

            string original = value.ToString();
            var mi = typeof(T).GetMember(original).First();
            var dv = mi.GetCustomAttribute<DefaultValueAttribute>();
            if (dv != null)
                original = dv.Value.ToString();
            if (IsValidIdentifier(original))
            {
                // We are dealing with a combination of flag constants since
                // all constant values were cached during warm-up.
                JavaScriptEncoder? encoder = options.Encoder;

                if (_nameCache.Count < NameCacheSizeSoftLimit)
                {
                    formatted = JsonEncodedText.Encode(original, encoder);
                    writer.WriteStringValue(formatted);
                    _nameCache.TryAdd(key, formatted);
                }
                else
                {
                    // We also do not create a JsonEncodedText instance here because passing the string
                    // directly to the writer is cheaper than creating one and not caching it for reuse.
                    writer.WriteStringValue(original);
                }

                return;
            }

            throw new JsonException("cannot encode enum value as string");
        }

        // This method is adapted from Enum.ToUInt64 (an internal method):
        // https://github.com/dotnet/runtime/blob/bd6cbe3642f51d70839912a6a666e5de747ad581/src/libraries/System.Private.CoreLib/src/System/Enum.cs#L240-L260
        private static ulong ConvertToUInt64(object value)
        {
            ulong result = EnumTypeCode switch
            {
                TypeCode.Int32 => (ulong)(int)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.SByte => (ulong)(sbyte)value,
                TypeCode.Byte => (byte)value,
                TypeCode.Int16 => (ulong)(short)value,
                TypeCode.UInt16 => (ushort)value,
                _ => throw new InvalidOperationException(),
            };
            return result;
        }

        private static bool IsValidIdentifier(string value)
        {
            // Trying to do this check efficiently. When an enum is converted to
            // string the underlying value is given if it can't find a matching
            // identifier (or identifiers in the case of flags).
            //
            // The underlying value will be given back with a digit (e.g. 0-9) possibly
            // preceded by a negative sign. Identifiers have to start with a letter
            // so we'll just pick the first valid one and check for a negative sign
            // if needed.
            return (value[0] >= 'A' &&
                (NegativeSign == null || !value.StartsWith(NegativeSign)));
        }

        internal T ReadWithQuotes(ref Utf8JsonReader reader)
        {
            string? enumString = reader.GetString();

            // Try parsing case sensitive first
            if (!Enum.TryParse(enumString, out T value)
                && !Enum.TryParse(enumString, ignoreCase: true, out value))
            {
                throw new JsonException("unable to parse string value to enum");
            }

            return value;
        }
    }
}