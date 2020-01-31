namespace TemplateBuilder.Core
{
	using System.IO;
	using Microsoft.Extensions.FileSystemGlobbing;
	using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
	using TemplateBuilder.Core.Models.Config;

	public static class FileReader
	{
		public static PatternMatchingResult GetFiles(string directory, TemplateConfig config)
		{
			var matcher = new Matcher();
			foreach (var file in config.Files)
			{
				matcher.AddInclude(file.Glob);
			}
			var info = new DirectoryInfoWrapper(new DirectoryInfo(directory));
			return matcher.Execute(info);
		}
	}
}
