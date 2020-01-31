namespace TemplateBuilder.ConsoleApp
{
	using System;
	using System.Collections.Generic;
	using McMaster.Extensions.CommandLineUtils;
	using TemplateBuilder.Core;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	internal static class Program
	{
		private static int Main(string[] args)
		{
			var app = new CommandLineApplication();

			app.HelpOption();
			//var optionPath = app.Option("-p|--path <PATH>", "The path", CommandOptionType.SingleValue).IsRequired();

			app.OnExecuteAsync(async (_) =>
			{
				//var path = optionPath.Value();
				const string path = @"C:\Users\clarkero\Documents\projects\dotnet\TestFolder";
				var prompts = await PromptReader.GetPromptsFromFile(path).ConfigureAwait(false);
				var results = ConsolePromptReader.WritePrompts(prompts);
				var config = await ConfigReader.GetConfigFromFile(path, results).ConfigureAwait(false);
				var files = FileReader.GetFiles(path, config);
				return 0;
			});

			return app.Execute(args);
		}
	}

	public static class ConsolePromptReader
	{
		public static IDictionary<string, object> WritePrompts(IDictionary<string, AbstractPrompt> prompts)
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
