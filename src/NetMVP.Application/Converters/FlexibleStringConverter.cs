using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetMVP.Application.Converters;

/// <summary>
/// 灵活的字符串转换器，支持将数字自动转换为字符串
/// 用于兼容前端发送数字类型但后端期望字符串类型的场景
/// </summary>
public class FlexibleStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                // 将数字转换为字符串
                if (reader.TryGetInt32(out int intValue))
                {
                    return intValue.ToString();
                }
                if (reader.TryGetInt64(out long longValue))
                {
                    return longValue.ToString();
                }
                if (reader.TryGetDouble(out double doubleValue))
                {
                    return doubleValue.ToString();
                }
                return reader.GetDecimal().ToString();
            case JsonTokenType.True:
                return "1";
            case JsonTokenType.False:
                return "0";
            case JsonTokenType.Null:
                return null;
            default:
                throw new JsonException($"无法将 {reader.TokenType} 转换为字符串");
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
