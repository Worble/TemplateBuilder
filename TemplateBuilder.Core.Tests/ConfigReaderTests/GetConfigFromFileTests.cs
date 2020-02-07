namespace TemplateBuilder.Core.Tests.ConfigReaderTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.Json;
	using System.Threading.Tasks;
	using TemplateBuilder.Core.Models.Config;
	using TemplateBuilder.Core.Tests.Abstract;
	using Xunit;

	public sealed class GetConfigFromFileTests : FileTestBase
	{
		internal override string Filename => "templateConfig.json";

		[Fact]
		public async Task GivenAnEmptyDirectoryParameter_WhenGetConfigFromFileIsCalled_ThenAnArgumentExceptionIsThrown()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();

			//act
			await Assert.ThrowsAsync<ArgumentException>(
				() => ConfigReader.GetConfigFromFile("", dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenNoValidFile_WhenGetConfigFromFileIsCalled_ThenAFileNotFoundExceptionIsThrown()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();

			//act
			await Assert.ThrowsAsync<FileNotFoundException>(
				() => ConfigReader.GetConfigFromFile(TempPath, dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenValidFileButAnInvalidName_WhenGetConfigFromFileIsCalled_ThenAFileNotFoundExceptionIsThrown()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();
			const string json = @"
{
	""files"": [
		{
			""glob"": ""index.html""
		}
	]
}";
			await File.WriteAllTextAsync(Path.Join(
				TempPath,
				Guid.NewGuid().ToString()), json).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<FileNotFoundException>(
				() => ConfigReader.GetConfigFromFile(TempPath, dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAValidFileWithADifferentName_WhenGetConfigFromFileIsCalledWithTheFilename_ThenTheExpectedObjectIsReturned()
		{
			//arrange
			var filename = Guid.NewGuid().ToString();
			var dictionary = new Dictionary<string, object>();
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "index.html"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""index.html""
		}
	]
}";
			await File.WriteAllTextAsync(Path.Join(
				TempPath,
				filename), json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(
				TempPath,
				dictionary,
				filename).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Theory]
		[InlineData(new object[] { "[]" })]
		[InlineData(new object[] { @"[""test""]" })]
		[InlineData(new object[] { @"{""test""}" })]
		[InlineData(new object[] { @"{""files"": ""test""}" })]
		[InlineData(new object[] { @"{""files"": {""test"": ""test""}}" })]
		[InlineData(new object[] { @"{""files"": [""test""]}" })]
		[InlineData(new object[] { @"{""files"": [""{""test"": ""test""}""]}" })]
		public async Task GivenAnInvalidFile_WhenGetConfigFromFileIsCalled_ThenAJsonExceptionIsThrown(string json)
		{
			//arrange
			var dictionary = new Dictionary<string, object>();
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<JsonException>(
				() => ConfigReader.GetConfigFromFile(TempPath, dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAValidFileAndAnEmptyDictionary_WhenGetConfigFromFileIsCalled_ThenTheExpectedObjectIsReturned()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "index.html"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""index.html""
		}
	]
}";
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(TempPath, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidFileWithAVariableAndAnEmptyDictionary_WhenGetConfigFromFileIsCalled_ThenTheValueIsReturnedWithoutTheVariable()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "/index.html"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""{{projectName}}/index.html""
		}
	]
}";
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(TempPath, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidFile_WithAVariable_AndADictionaryWithTheVariableEntry_WhenGetConfigFromFileIsCalled_ThenTheExpectedObjectIsReturned()
		{
			//arrange
			var dictionary = new Dictionary<string, object>() { { "projectName", "myProject" } };
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "myProject/index.html"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""{{projectName}}/index.html""
		}
	]
}";
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(TempPath, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidFile_WithASection_AndADictionaryWithoutTheSectionVariableEntry_WhenGetConfigFromFileIsCalled_ThenTheSectionIsntRendered()
		{
			//arrange
			var dictionary = new Dictionary<string, object>()
			{
				{ "projectName", "myProject" }
			};
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "myProject/index.html"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""{{projectName}}/index.html""
		}{{#newSection}},
		{
			""glob"": ""{{projectName}}/index.js""
		}
		{{/newSection}}
	]
}";
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(TempPath, dictionary).ConfigureAwait(false);

			//assert
			Assert.Single(expected.Files);
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidFile_WithASection_AndADictionaryWithTheSectionVariableEntry_WhenGetConfigFromFileIsCalled_ThenTheExpectedObjectIsReturned()
		{
			//arrange
			const int expectedFileCount = 2;
			var dictionary = new Dictionary<string, object>()
			{
				{ "projectName", "myProject" },
				{ "newSection", true }
			};
			var expected = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
					{
						new TemplateFileConfig
						{
							Glob = "myProject/index.html"
						},
						new TemplateFileConfig
						{
							Glob = "myProject/index.js"
						}
					}
			};
			const string json = @"
{
	""files"": [
		{
			""glob"": ""{{projectName}}/index.html""
		}{{#newSection}},
		{
			""glob"": ""{{projectName}}/index.js""
		}
		{{/newSection}}
	]
}";
			await File.WriteAllTextAsync(TempFile, json).ConfigureAwait(false);

			//act
			var result = await ConfigReader.GetConfigFromFile(TempPath, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expectedFileCount, expected.Files.Count);
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
			Assert.Equal(expected.Files[1].Glob, result.Files[1].Glob);
		}
	}
}
