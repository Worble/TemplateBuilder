using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;
using TemplateBuilder.Core;

namespace TemplateBuilder.ConsoleApp
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			var app = new CommandLineApplication();

			app.HelpOption();
			//var optionPath = app.Option("-p|--path <PATH>", "The path", CommandOptionType.SingleValue).IsRequired();

			app.OnExecuteAsync(async (token) =>
			{
				//var path = optionPath.Value();
				var path = @"C:\Users\clarkero\Documents\projects\dotnet\TestFolder";
				var prompts = await ReadPromptsClass.GetPrompts(path).ConfigureAwait(false);
				var consolePromptReader = new ConsolePromptReader(prompts, PromptHandlers());
				consolePromptReader.WritePrompts();
				return 0;
			});

			return app.Execute(args);
		}

		private static List<ITemplatePromptHandler> PromptHandlers()
		{
			return new List<ITemplatePromptHandler>()
			{
				new BooleanPromptHandler(),
				new StringPromptHandler(),
				new IntPromptHandler()
			};
		}
	}

	internal class ConsolePromptReader
	{
		private readonly IEnumerable<ITemplatePrompt> _prompts = new List<ITemplatePrompt>();
		private readonly IEnumerable<ITemplatePromptHandler> _promptHandlers;

		public ConsolePromptReader(IEnumerable<ITemplatePrompt> prompts, IEnumerable<ITemplatePromptHandler> promptHandlers)
		{
			_prompts = prompts;
			_promptHandlers = promptHandlers;
		}

		public void WritePrompts()
		{
			foreach (var prompt in _prompts)
			{
				GetHandlerForPrompt(prompt);
			}
		}

		private void GetHandlerForPrompt<T>(T prompt) where T : ITemplatePrompt
		{
			_promptHandlers.OfType<ITemplatePromptHandler<T>>().First().HandlePrompt(prompt);
			//return _promptHandlers.First(e => e.Type == prompt.GetType());
		}
	}

	public interface ITemplatePromptHandler { }

	public interface ITemplatePromptHandler<T> : ITemplatePromptHandler where T : ITemplatePrompt
	{
		public T HandlePrompt(T prompt);
	}

	public class BooleanPromptHandler : ITemplatePromptHandler<BooleanPrompt>
	{
		public BooleanPrompt HandlePrompt(BooleanPrompt prompt)
		{
			prompt.Value = Prompt.GetYesNo(prompt.Message, prompt.Value);
			return prompt;
		}
	}

	public class StringPromptHandler : ITemplatePromptHandler<StringPrompt>
	{
		public StringPrompt HandlePrompt(StringPrompt prompt)
		{
			prompt.Value = Prompt.GetString(prompt.Message, prompt.Value);
			return prompt;
		}
	}

	public class IntPromptHandler : ITemplatePromptHandler<IntPrompt>
	{
		public IntPrompt HandlePrompt(IntPrompt prompt)
		{
			prompt.Value = Prompt.GetInt(prompt.Message, prompt.Value);
			return prompt;
		}
	}
}
