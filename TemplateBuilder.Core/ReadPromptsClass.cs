using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TemplateBuilder.Core
{
	public static class ReadPromptsClass
	{
		public static async Task<List<TemplatePrompt>> GetPrompts(string directory)
		{
			List<DeserializedTemplatePrompt> serializedPrompts = await GetPromptsFromFile(directory).ConfigureAwait(false);

			var prompts = new List<TemplatePrompt>();
			foreach (var prompt in serializedPrompts)
			{
				prompts.Add(GetPromptForType(prompt));
			}

			return prompts;
		}

		private static async Task<List<DeserializedTemplatePrompt>> GetPromptsFromFile(string directory)
		{
			using var stream = new FileStream(Path.Join(directory, "prompts.json"), FileMode.Open);
			var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			jsonOpts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			return await JsonSerializer
				.DeserializeAsync<List<DeserializedTemplatePrompt>>(stream, jsonOpts)
				.AsTask()
				.ConfigureAwait(false);
		}

		private static TemplatePrompt GetPromptForType(DeserializedTemplatePrompt prompt)
		{
			return prompt.PromptType switch
			{
				PromptType.Boolean => (BooleanPrompt)prompt,
				PromptType.String => (StringPrompt)prompt,
				PromptType.Int => (IntPrompt)prompt,
				_ => throw new InvalidPromptTypeException(),
			};
		}
	}

	[Serializable]
	internal class InvalidPromptTypeException : Exception
	{
		public InvalidPromptTypeException()
		{
		}

		public InvalidPromptTypeException(string message) : base(message)
		{
		}

		public InvalidPromptTypeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPromptTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

	public class DeserializedTemplatePrompt
	{
		public PromptType PromptType { get; set; }
		public string Id { get; set; }
		public string Message { get; set; }
		public object DefaultValue { get; set; }
	}

	public interface ITemplatePrompt { }

	public abstract class TemplatePrompt : ITemplatePrompt
	{
		public string Id { get; set; }
		public string Message { get; set; }
	}

	public class BooleanPrompt : TemplatePrompt
	{
		public bool Value { get; set; }

		public static implicit operator BooleanPrompt(DeserializedTemplatePrompt template) =>
			new BooleanPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue != null
					? template.DefaultValue is bool value
						? value
						: bool.Parse(template.DefaultValue.ToString())
					: default(bool)
			};
	}

	public class StringPrompt : TemplatePrompt
	{
		public string Value { get; set; }

		public static implicit operator StringPrompt(DeserializedTemplatePrompt template) =>
			new StringPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue?.ToString() ?? default
			};
	}

	public class IntPrompt : TemplatePrompt
	{
		public int Value { get; set; }

		public static implicit operator IntPrompt(DeserializedTemplatePrompt template) =>
			new IntPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue != null
					? template.DefaultValue is int value
						? value
						: int.Parse(template.DefaultValue.ToString())
					: default(int)
			};
	}

	public enum PromptType
	{
		Boolean,
		String,
		Int
	}
}
