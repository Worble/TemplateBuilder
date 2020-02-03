namespace TemplateBuilder.ConsoleApp
{
	using System;
	using System.Collections.Generic;
	using McMaster.Extensions.CommandLineUtils;
	using TemplateBuilder.Core;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	public static class ConsolePromptReader
	{
		public static IDictionary<string, object> WritePrompts(IEnumerable<KeyValuePair<string, AbstractPrompt>> prompts)
		{
			var dictionary = new Dictionary<string, object>();
			foreach (var prompt in prompts)
			{
				switch (prompt.Value)
				{
					case BooleanPrompt booleanPrompt:
						dictionary.Add(
							prompt.Key,
							Prompt.GetYesNo(booleanPrompt.Message, booleanPrompt.Value));
						break;

					case StringPrompt stringPrompt:
						dictionary.Add(
							prompt.Key,
							Prompt.GetString(stringPrompt.Message, stringPrompt.Value) ?? string.Empty);
						break;

					case IntPrompt intPrompt:
						dictionary.Add(
							prompt.Key,
							Prompt.GetInt(intPrompt.Message, intPrompt.Value));
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
