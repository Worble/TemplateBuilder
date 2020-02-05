namespace TemplateBuilder.Core.Tests.FileProcessorTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using TemplateBuilder.Core.Models.Config;
	using Xunit;

	public sealed class GetFilesToMoveTests : IDisposable
	{
		private static string TempPath { get; } = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

		public GetFilesToMoveTests()
		{
			if (Directory.Exists(TempPath))
			{
				Directory.Delete(TempPath, true);
			}
			Directory.CreateDirectory(TempPath);
		}

		public void Dispose()
		{
			if (Directory.Exists(TempPath))
			{
				Directory.Delete(TempPath, true);
			}
		}

		[Fact]
		public void GivenAnEmptyDirectoryParameter_WhenGetFilesToMoveIsCalled_ThenAnArgumentExceptionWillBeThrown()
		{
			//arrange
			var config = new TemplateConfig();
			var prompts = new Dictionary<string, object>();

			//act
			Assert.Throws<ArgumentException>(() => FileProcessor.GetFilesToMove("", config, prompts));

			//assert
		}

		[Fact]
		public void GivenATemplateWithNofiles_WhenGetFilesToMoveIsCalled_ThenAnArgumentExceptionWillBeThrown()
		{
			//arrange
			var config = new TemplateConfig();
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.False(result.Any());
		}
	}
}
