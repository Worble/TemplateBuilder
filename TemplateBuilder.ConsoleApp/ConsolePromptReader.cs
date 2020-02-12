namespace TemplateBuilder.ConsoleApp
{
	using System;
	using System.Collections.Generic;
	using McMaster.Extensions.CommandLineUtils;
	using TemplateBuilder.Core.Enums;
	using TemplateBuilder.Core.Models.Prompts;

	public static class ConsolePromptReader
	{
		/// <summary>
		/// Writes the prompts to the console for input.
		/// </summary>
		/// <param name="prompts">The prompts.</param>
		/// <returns>An <see cref="Dictionary{TKey, TValue}" of responses/></returns>
		/// <exception cref="ArgumentException"></exception>
		public static Dictionary<string, object> WritePrompts(IEnumerable<TemplatePrompt> prompts)
		{
			var dictionary = new Dictionary<string, object>();
			foreach (var prompt in prompts)
			{
				switch (prompt.PromptType)
				{
					case PromptType.Boolean:
						dictionary.Add(
							prompt.Id,
							Prompt.GetYesNo(prompt.Message, prompt.GetBoolValue()));
						break;

					case PromptType.String:
						dictionary.Add(
							prompt.Id,
							Prompt.GetString(
								prompt.Message, prompt.GetStringValue()) ?? string.Empty);
						break;

					case PromptType.Int:
						dictionary.Add(
							prompt.Id,
							Prompt.GetInt(prompt.Message, prompt.GetIntValue()));
						break;

					default:
						throw new ArgumentException(
							message: "No Handler For Prompt",
							paramName: nameof(prompt));
				}
			}
			return dictionary;
		}
	}
}
