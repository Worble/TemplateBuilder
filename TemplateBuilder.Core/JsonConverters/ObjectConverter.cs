namespace TemplateBuilder.Core.JsonConverters
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	internal class ObjectConverter : JsonConverter<object>
	{
		public override object Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.True)
			{
				return true;
			}

			if (reader.TokenType == JsonTokenType.False)
			{
				return false;
			}

			if (options.GetConverter(typeof(JsonElement)) is JsonConverter<JsonElement> converter)
			{
				if (reader.TokenType == JsonTokenType.Number)
				{
					return converter.Read(ref reader, type, options).GetInt32();
				}
				if (reader.TokenType == JsonTokenType.String)
				{
					return converter.Read(ref reader, type, options).GetString();
				}
				return converter.Read(ref reader, type, options);
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			throw new InvalidOperationException("Directly writing object not supported");
		}
	}
}
