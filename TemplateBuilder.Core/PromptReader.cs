namespace TemplateBuilder.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.Json;
	using System.Threading.Tasks;
	using FluentValidation;
	using TemplateBuilder.Core.Helpers;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Validators;

	public static class PromptReader
	{
		#region Properties

		private const string JSON_FILENAME = "templatePrompts.json";

		#endregion Properties

		#region Public Methods

		/// <summary>Reads the prompts from the specified location</summary>
		/// <param name="directory">The directory to read from</param>
		/// <param name="fileName">Optional filename if the file is not named templatePrompts.json</param>
		/// <returns>An IEnumerable of <see cref="TemplatePrompt"/></returns>
		/// <exception cref="ValidationException" />
		/// <exception cref="FileNotFoundException" />
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static async Task<IEnumerable<TemplatePrompt>> GetPromptsFromFile(string directory, string fileName = JSON_FILENAME)
		{
			if (string.IsNullOrWhiteSpace(directory))
			{
				throw new ArgumentException("Directory cannot be null or empty string", nameof(directory));
			}

			var serializedPrompts = await DeserializePromptsFromFile(directory, fileName).ConfigureAwait(false);

			ValidatePrompts(serializedPrompts);

			return serializedPrompts;
		}

		/// <summary>Reads the prompts from the provided JSON string</summary>
		/// <param name="json">The JSON string to convert from</param>
		/// <returns>An IEnumerable of <see cref="TemplatePrompt"/></returns>
		/// <exception cref="ValidationException" />
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static IEnumerable<TemplatePrompt> GetPromptsFromString(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentException("Directory cannot be null or empty string", nameof(json));
			}

			var serializedPrompts = JsonHelper.Deserialize<IEnumerable<TemplatePrompt>>(json);

			ValidatePrompts(serializedPrompts);

			return serializedPrompts;
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>Reads the prompts from the json file in the specified directory</summary>
		/// <param name="directory">The directory to read from</param>
		/// <param name="filename"></param>
		/// <returns>A list of prompts</returns>
		/// <see cref="TemplatePrompt"/>
		/// <exception cref="FileNotFoundException" />
		/// <exception cref="JsonException" />
		private static Task<IEnumerable<TemplatePrompt>> DeserializePromptsFromFile(string directory, string filename)
		{
			var filePath = Path.Join(directory, filename);
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Prompt file does not exist on path {filePath}");
			}
			return JsonHelper.DeserializeFromFile<IEnumerable<TemplatePrompt>>(filePath);
		}

		/// <summary>Validates the prompts</summary>
		/// <param name="serializedPrompts"></param>
		/// <exception cref="ValidationException" />
		private static void ValidatePrompts(IEnumerable<TemplatePrompt> serializedPrompts)
		{
			foreach (var prompt in serializedPrompts)
			{
				var validator = new TemplatePromptValidator();
				var result = validator.Validate(prompt);
				if (!result.IsValid)
				{
					throw new ValidationException(result.Errors);
				}
			}
		}

		#endregion Private Methods
	}
}
