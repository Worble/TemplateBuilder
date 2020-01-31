namespace TemplateBuilder.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.Json;
	using System.Threading.Tasks;
	using FluentValidation;
	using TemplateBuilder.Core.Enums;
	using TemplateBuilder.Core.Exceptions;
	using TemplateBuilder.Core.Helpers;
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Models.Prompts.Abstract;
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
		/// <returns>A Dictionary of <see cref="AbstractPrompt"/> with the <see cref="AbstractPrompt.Id"/> parameter as the key</returns>
		/// <exception cref="ValidationException" />
		/// <exception cref="FileNotFoundException" />
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static async Task<IDictionary<string, AbstractPrompt>> GetPromptsFromFile(string directory, string fileName = JSON_FILENAME)
		{
			if (string.IsNullOrWhiteSpace(directory))
			{
				throw new ArgumentException("Directory cannot be null or empty string", nameof(directory));
			}

			var serializedPrompts = await GetDeserializedTemplatePromptsFromFile(directory, fileName).ConfigureAwait(false);

			ValidatePrompts(serializedPrompts);

			return ConvertPrompts(serializedPrompts);
		}

		/// <summary>Reads the prompts from the provided JSON string</summary>
		/// <param name="json">The JSON string to convert from</param>
		/// <returns>A Dictionary of <see cref="AbstractPrompt"/> with the <see cref="AbstractPrompt.Id"/> parameter as the key</returns>
		/// <exception cref="ValidationException" />
		/// <exception cref="JsonException" />
		/// <exception cref="ArgumentException" />
		public static IDictionary<string, AbstractPrompt> GetPromptsFromString(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentException("Directory cannot be null or empty string", nameof(json));
			}

			List<DeserializedTemplatePrompt> serializedPrompts = GetDeserializedTemplatePromptsFromString(json);

			ValidatePrompts(serializedPrompts);

			return ConvertPrompts(serializedPrompts);
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>Reads the prompts from the provided JSON string</summary>
		/// <param name="json">The JSON to convert from</param>
		/// <returns>A list of <see cref="DeserializedTemplatePrompt"/></returns>
		/// <exception cref="JsonException" />
		private static List<DeserializedTemplatePrompt> GetDeserializedTemplatePromptsFromString(string json)
		{
			return JsonSerializer.Deserialize<List<DeserializedTemplatePrompt>>(json, JsonHelper.GetJsonOptions());
		}

		/// <summary>Converts all prompts to their appropriate underlying type</summary>
		/// <param name="serializedPrompts"></param>
		/// <returns>A Dictionary of <see cref="AbstractPrompt"/> with the Id parameter as the key</returns>
		private static Dictionary<string, AbstractPrompt> ConvertPrompts(List<DeserializedTemplatePrompt> serializedPrompts)
		{
			var prompts = new Dictionary<string, AbstractPrompt>();
			foreach (var prompt in serializedPrompts)
			{
				prompts.Add(prompt.Id, GetPromptForType(prompt));
			}

			return prompts;
		}

		/// <summary>Reads the prompts from the json file in the specified directory</summary>
		/// <param name="directory">The directory to read from</param>
		/// <param name="filename"></param>
		/// <returns>A list of deserialized prompts</returns>
		/// <see cref="DeserializedTemplatePrompt"/>
		/// <exception cref="FileNotFoundException" />
		private static async Task<List<DeserializedTemplatePrompt>> GetDeserializedTemplatePromptsFromFile(string directory, string filename)
		{
			var filePath = Path.Join(directory, filename);
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"File does not exist on path {filePath}");
			}
			using var stream = new FileStream(filePath, FileMode.Open);
			return await JsonSerializer
				.DeserializeAsync<List<DeserializedTemplatePrompt>>(stream, JsonHelper.GetJsonOptions())
				.ConfigureAwait(false);
		}

		/// <summary>Validates the deserialized prompts</summary>
		/// <param name="serializedPrompts"></param>
		/// <exception cref="ValidationException" />
		private static void ValidatePrompts(List<DeserializedTemplatePrompt> serializedPrompts)
		{
			foreach (var prompt in serializedPrompts)
			{
				var validator = new DeserializedTemplatePromptValidator();
				var result = validator.Validate(prompt);
				if (!result.IsValid)
				{
					throw new ValidationException(result.Errors);
				}
			}
		}

		/// <summary>Casts the deserialized into the appropriate underlying type</summary>
		/// <param name="prompt"></param>
		/// <returns><see cref="AbstractPrompt"/></returns>
		private static AbstractPrompt GetPromptForType(DeserializedTemplatePrompt prompt)
		{
			return prompt.PromptType switch
			{
				PromptType.Boolean => (BooleanPrompt)prompt,
				PromptType.String => (StringPrompt)prompt,
				PromptType.Int => (IntPrompt)prompt,
				_ => throw new InvalidPromptTypeException(),
			};
		}

		#endregion Private Methods
	}
}
