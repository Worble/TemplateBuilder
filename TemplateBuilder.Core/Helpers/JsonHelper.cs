namespace TemplateBuilder.Core.Helpers
{
	using System;
	using System.IO;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Threading.Tasks;

	public static class JsonHelper
	{
		/// <summary>
		/// Deserializes the JSON string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json">The JSON string.</param>
		/// <returns><see cref="T"/></returns>
		/// <exception cref="JsonException" />
		public static T Deserialize<T>(string json)
		{
			var jsonOpts = GetJsonOptions();
			return JsonSerializer.Deserialize<T>(json, jsonOpts);
		}

		/// <summary>
		/// Deserializes JSON from a file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">The file path.</param>
		/// <returns><see cref="T"/></returns>
		/// <exception cref="JsonException" />
		public async static Task<T> DeserializeFromFile<T>(string filePath)
		{
			using var stream = new FileStream(filePath, FileMode.Open);
			return await JsonSerializer
				.DeserializeAsync<T>(stream, GetJsonOptions())
				.ConfigureAwait(false);
		}

		/// <summary>Gets the Json Serializer Options</summary>
		/// <returns><see cref="JsonSerializerOptions"/></returns>
		private static JsonSerializerOptions GetJsonOptions()
		{
			var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			jsonOpts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			jsonOpts.Converters.Add(new ObjectConverter());
			return jsonOpts;
		}
	}

	public class ObjectConverter : JsonConverter<object>
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

			// Forward to the JsonElement converter
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

			// or for best performance, copy-paste the code from that converter:
			//using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			//{
			//    return document.RootElement.Clone();
			//}
		}

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			throw new InvalidOperationException("Directly writing object not supported");
		}
	}
}
