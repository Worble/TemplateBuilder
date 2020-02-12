namespace TemplateBuilder.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.Json;
	using System.Threading.Tasks;
	using TemplateBuilder.Core.Helpers;
	using TemplateBuilder.Core.Models.Config;

	public static class ConfigReader
	{
		#region Properties

		public const string JSON_FILENAME = "templateConfig.json";

		#endregion Properties

		#region Public Methods

		/// <summary>Reads the config from the provided JSON string</summary>
		/// <param name="json">The JSON string to convert from</param>
		/// <param name="promptResults">A <see cref="IDictionary{TKey, TValue}"/> of values from the prompts, where the key is the <see cref="AbstractPrompt.Id"/> and Value is the prompt result</param>
		/// <returns><see cref="TemplateConfig"/></returns>
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static async Task<TemplateConfig> GetConfigFromString(string json, IDictionary<string, object> promptResults)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentException("JSON string cannot be null or empty string", nameof(json));
			}

			var output = await MoustacheHelper.ApplyMoustache(json, promptResults).ConfigureAwait(false);

			return JsonHelper.Deserialize<TemplateConfig>(output);
		}

		/// <summary>Reads the config from the provided file</summary>
		/// <param name="directory">The folder containing the config file</param>
		/// <param name="results">A <see cref="IDictionary{TKey, TValue}"/> of values from the prompts, where the key is the <see cref="AbstractPrompt.Id"/> and Value is the prompt result</param>
		/// <param name="filename">Optional filename if the file is not named templateConfig.json</param>
		/// <returns><see cref="TemplateConfig"/></returns>
		/// <exception cref="FileNotFoundException" />
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static async Task<TemplateConfig> GetConfigFromFile(string directory, IDictionary<string, object> results, string filename = JSON_FILENAME)
		{
			if (string.IsNullOrWhiteSpace(directory))
			{
				throw new ArgumentException("Directory cannot be null or empty string", nameof(directory));
			}
			var filePath = Path.Join(directory, filename);
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Config file does not exist on path {filePath}");
			}

			var content = string.Empty;
			using (var stream = new StreamReader(filePath, Encoding.UTF8))
			{
				content = await stream.ReadToEndAsync().ConfigureAwait(false);
			}
			var output = await MoustacheHelper.ApplyMoustache(content, results).ConfigureAwait(false);

			return JsonHelper.Deserialize<TemplateConfig>(output);
		}

		#endregion Public Methods
	}
}
