namespace TemplateBuilder.Core.Helpers
{
	using System.Text.Json;
	using System.Text.Json.Serialization;

	public static class JsonHelper
	{
		/// <summary>Gets the Json Serializer Options</summary>
		/// <returns><see cref="JsonSerializerOptions"/></returns>
		public static JsonSerializerOptions GetJsonOptions()
		{
			var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			jsonOpts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			return jsonOpts;
		}
	}
}
