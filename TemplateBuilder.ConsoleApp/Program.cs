namespace TemplateBuilder.ConsoleApp
{
	using System;
	using McMaster.Extensions.CommandLineUtils;
	using TemplateBuilder.Core;

	internal static class Program
	{
		private static int Main(string[] args)
		{
			var app = new CommandLineApplication();

			app.HelpOption();

			var originPathOption = app.Option("-p|--path <PATH>", "The origin path", CommandOptionType.SingleValue).IsRequired();
			var destinationPathOption = app.Option("-d|--destination <PATH>", "[optional] The destination path. Defaults to the current working directory", CommandOptionType.SingleValue);

			app.OnExecuteAsync(async (cancellationToken) =>
			{
				var originPath = originPathOption.Value() ?? throw new InvalidOperationException();
				var destinationPath = destinationPathOption.Value() ?? Environment.CurrentDirectory;
				PrintOpener(originPath, destinationPath);
				var prompts = await PromptReader.GetPromptsFromFile(originPath).ConfigureAwait(false);
				var promptResults = ConsolePromptReader.WritePrompts(prompts);
				var config = await ConfigReader.GetConfigFromFile(originPath, promptResults).ConfigureAwait(false);
				Console.WriteLine("Moving files...");
				var filesResult = FileProcessor.GetFilesToMove(originPath, config, promptResults);
				await FileProcessor.MoveFiles(originPath, destinationPath, filesResult, cancellationToken).ConfigureAwait(false);
				Console.WriteLine("All done!");
				return 0;
			});

			return app.Execute(args);
		}

		private static void PrintOpener(string originPath, string destinationPath)
		{
			Console.WriteLine("=================================");
			Console.WriteLine($"Directory to read template from: {originPath}");
			Console.WriteLine($"Directory to write to template to: {destinationPath}");
			Console.WriteLine("=================================");
		}
	}
}
