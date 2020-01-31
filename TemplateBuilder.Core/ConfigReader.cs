namespace TemplateBuilder.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.Json;
	using System.Threading.Tasks;
	using Stubble.Core.Builders;
	using TemplateBuilder.Core.Helpers;
	using TemplateBuilder.Core.Models.Config;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	public static class ConfigReader
	{
		#region Properties

		public const string JSON_FILENAME = "templateConfig.json";

		#endregion Properties

		#region Public Methods

		/// <summary>Reads the config from the provided JSON string</summary>
		/// <param name="json">The JSON string to convert from</param>
		/// <param name="results">A <see cref="IDictionary{TKey, TValue}"/> of values from the prompts, where the key is the <see cref="AbstractPrompt.Id"/> and Value is the prompt result</param>
		/// <returns><see cref="TemplateConfig"/></returns>
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static async Task<TemplateConfig> GetConfigFromString(string json, IDictionary<string, object> results)
		{
			var output = await ApplyMoutacheToConfig(json, results).ConfigureAwait(false);

			return DeserializeConfig(output);
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
				throw new FileNotFoundException($"File does not exist on path {filePath}");
			}

			var content = string.Empty;
			using (var stream = new StreamReader(filePath, Encoding.UTF8))
			{
				content = await stream.ReadToEndAsync().ConfigureAwait(false);
			}
			var output = await ApplyMoutacheToConfig(content, results).ConfigureAwait(false);
			return DeserializeConfig(output);
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Deserializes the configuration string.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <returns><see cref="TemplateConfig"/></returns>
		/// <exception cref="JsonException" />
		private static TemplateConfig DeserializeConfig(string configuration)
		{
			var jsonOpts = JsonHelper.GetJsonOptions();
			return JsonSerializer.Deserialize<TemplateConfig>(configuration, jsonOpts);
		}

		/// <summary>
		/// Applies the moutache rendering to the configuration string.
		/// </summary>
		/// <param name="configuration">The content.</param>
		/// <param name="results">The prompt results.</param>
		/// <returns>The configuration with moustache rendering applied</returns>
		private static async Task<string> ApplyMoutacheToConfig(string configuration, IDictionary<string, object> results)
		{
			var stubble = new StubbleBuilder().Build();
			return await stubble
				.RenderAsync(configuration, results)
				.ConfigureAwait(false);
		}

		#endregion Private Methods
	}
}
