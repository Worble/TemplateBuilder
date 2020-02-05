namespace TemplateBuilder.Core.Tests.ConfigReaderTests
{
	using System;
	using System.Collections.Generic;
	using System.Text.Json;
	using System.Threading.Tasks;
	using TemplateBuilder.Core.Models.Config;
	using Xunit;

	public class GetConfigFromStringTests
	{
		[Fact]
		public async Task GivenAnEmptyString_WhenGetConfigFromStringIsCalled_ThenAnArgumentExceptionIsThrown()
		{
			//arrange
			var dictionary = new Dictionary<string, object>();

			//act
			await Assert.ThrowsAsync<ArgumentException>(
				() => ConfigReader.GetConfigFromString("", dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Theory]
		[InlineData(new object[] { "[]" })]
		[InlineData(new object[] { @"[""test""]" })]
		[InlineData(new object[] { @"{""test""}" })]
		[InlineData(new object[] { @"{""files"": ""test""}" })]
		[InlineData(new object[] { @"{""files"": {""test"": ""test""}}" })]
		[InlineData(new object[] { @"{""files"": [""test""]}" })]
		[InlineData(new object[] { @"{""files"": [""{""test"": ""test""}""]}" })]
		public async Task GivenAnInvalidString_WhenGetConfigFromStringIsCalled_ThenAJsonExceptionIsThrown(string json)
		{
			//arrange
			var dictionary = new Dictionary<string, object>();

			//act
			await Assert.ThrowsAsync<JsonException>(
				() => ConfigReader.GetConfigFromString(json, dictionary))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAValidStringAndAnEmptyDictionary_WhenGetConfigFromStringIsCalled_ThenTheExpectedObjectIsReturned()
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

			//act
			var result = await ConfigReader.GetConfigFromString(json, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidStringWithAVariableAndAnEmptyDictionary_WhenGetConfigFromStringIsCalled_ThenTheValueIsReturnedWithoutTheVariable()
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

			//act
			var result = await ConfigReader.GetConfigFromString(json, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidString_WithAVariable_AndADictionaryWithTheVariableEntry_WhenGetConfigFromStringIsCalled_ThenTheExpectedObjectIsReturned()
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

			//act
			var result = await ConfigReader.GetConfigFromString(json, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidString_WithASection_AndADictionaryWithoutTheSectionVariableEntry_WhenGetConfigFromStringIsCalled_ThenTheSectionIsntRendered()
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

			//act
			var result = await ConfigReader.GetConfigFromString(json, dictionary).ConfigureAwait(false);

			//assert
			Assert.Single(expected.Files);
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
		}

		[Fact]
		public async Task GivenAValidString_WithASection_AndADictionaryWithTheSectionVariableEntry_WhenGetConfigFromStringIsCalled_ThenTheExpectedObjectIsReturned()
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

			//act
			var result = await ConfigReader.GetConfigFromString(json, dictionary).ConfigureAwait(false);

			//assert
			Assert.Equal(expectedFileCount, expected.Files.Count);
			Assert.Equal(expected.Files[0].Glob, result.Files[0].Glob);
			Assert.Equal(expected.Files[1].Glob, result.Files[1].Glob);
		}
	}
}
