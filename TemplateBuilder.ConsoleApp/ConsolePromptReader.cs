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
		/// <returns>An <see cref="Dictionary{string, object}" of responses/></returns>
		/// <exception cref="ArgumentException"></exception>
		public static Dictionary<string, object> WritePrompts(List<TemplatePrompt> prompts)
		{
			//TODO: Rewrite all of this
			var promptResults = new Dictionary<string, object>();
			while (prompts.Count > 0)
			{
				var prompt = prompts[0];
				var shouldPrompt = true;
				var shouldSkip = false;
				if (prompt.When.Count > 0)
				{
					shouldPrompt = false;
					var whens = 0;
					foreach (var when in prompt.When)
					{
						if (!promptResults.TryGetValue(when.Id, out var val))
						{
							shouldSkip = true;
							break;
						}
						if (!val.Equals(when.Is))
						{
							continue;
						}
						whens++;
					}
					if (whens == prompt.When.Count)
					{
						shouldPrompt = true;
					}
				}
				if (shouldPrompt)
				{
					DisplayPrompt(promptResults, prompt);
				}
				else if (shouldSkip)
				{
					prompts.Add(prompt);
				}
				prompts.Remove(prompt);
			}
			return promptResults;
		}

		private static void DisplayPrompt(Dictionary<string, object> promptResults, TemplatePrompt prompt)
		{
			switch (prompt.PromptType)
			{
				case PromptType.Boolean:
					promptResults.Add(
						prompt.Id,
						Prompt.GetYesNo(prompt.Message, (bool)prompt.DefaultValue));
					break;

				case PromptType.String:
					promptResults.Add(
						prompt.Id,
						Prompt.GetString(
							prompt.Message, (string)prompt.DefaultValue) ?? string.Empty);
					break;

				case PromptType.Int:
					promptResults.Add(
						prompt.Id,
						Prompt.GetInt(prompt.Message, (int)prompt.DefaultValue));
					break;

				default:
					throw new ArgumentException(
						message: "No Handler For Prompt",
						paramName: nameof(prompt));
			}
		}
	}
}
