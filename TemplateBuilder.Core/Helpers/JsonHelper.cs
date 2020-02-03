namespace TemplateBuilder.Core.Helpers
{
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
			return jsonOpts;
		}
	}
}
