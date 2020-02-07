namespace TemplateBuilder.Core.Tests.FileProcessorTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using TemplateBuilder.Core.Models.Config;
	using TemplateBuilder.Core.Tests.Abstract;
	using Xunit;

	public sealed class GetFilesToMoveTests : FileTestBase
	{
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
		public void GivenATemplateWithNoGlobs_WhenGetFilesToMoveIsCalled_ThenAnArgumentExceptionWillBeThrown()
		{
			//arrange
			var config = new TemplateConfig();
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.False(result.Any());
		}

		[Fact]
		public async Task GivenATemplateWithOneGlob_AndAValidFileToMove_WhenGetFilesToMoveIsCalled_ThenOneFileWillBeFoundToMove()
		{
			//arrange
			const string expectedFilename = "index.html";
			await File.WriteAllTextAsync(Path.Join(TempPath, expectedFilename), string.Empty).ConfigureAwait(false);
			var config = new TemplateConfig { Files = new List<TemplateFileConfig> { new TemplateFileConfig { Glob = "index.html" } } };
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.Single(result.First().Files);
			Assert.Equal(expectedFilename, result.First().Files.First().Path);
		}

		[Fact]
		public async Task GivenATemplateWithOneGlob_AndNoValidFileToMove_WhenGetFilesToMoveIsCalled_ThenNoFilesWillBeFoundToMove()
		{
			//arrange
			const string expectedFilename = "DONOTMOVE.html";
			await File.WriteAllTextAsync(Path.Join(TempPath, expectedFilename), string.Empty).ConfigureAwait(false);
			var config = new TemplateConfig { Files = new List<TemplateFileConfig> { new TemplateFileConfig { Glob = "index.html" } } };
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.Empty(result.First().Files);
		}

		[Theory]
		[InlineData(new object[] { 2 })]
		[InlineData(new object[] { 3 })]
		[InlineData(new object[] { 5 })]
		[InlineData(new object[] { 8 })]
		public async Task GivenATemplateWithMultipleMatchingGlobs_AndValidFilesToMove_WhenGetFilesToMoveIsCalled_ThenAllTheFilesWillBeFound(int expectedAmount)
		{
			//arrange
			for (var i = 0; i < expectedAmount; i++)
			{
				var filename = $"test-file-{i}.html";
				await File.WriteAllTextAsync(Path.Join(TempPath, filename), string.Empty).ConfigureAwait(false);
			}

			var config = new TemplateConfig { Files = new List<TemplateFileConfig> { new TemplateFileConfig { Glob = "*.html" } } };
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.Equal(expectedAmount, result.First().Files.Count());
		}

		[Theory]
		[InlineData(new object[] { 2, 3 })]
		[InlineData(new object[] { 3, 5 })]
		[InlineData(new object[] { 5, 2 })]
		[InlineData(new object[] { 8, 4 })]
		public async Task GivenMultipleGlobsWithMatchingFiles_AndValidFilesToMove_WhenGetFilesToMoveIsCalled_ThenAllTheFilesWillBeFound(int globOneExpectedAmount, int globTwoExpectedAmount)
		{
			//arrange
			for (var i = 0; i < globOneExpectedAmount; i++)
			{
				var filename = $"test-file-{i}.html";
				await File.WriteAllTextAsync(Path.Join(TempPath, filename), string.Empty).ConfigureAwait(false);
			}

			for (var i = 0; i < globTwoExpectedAmount; i++)
			{
				var filename = $"test-file-{i}.css";
				await File.WriteAllTextAsync(Path.Join(TempPath, filename), string.Empty).ConfigureAwait(false);
			}

			var config = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
				{
					new TemplateFileConfig { Glob = "*.html" },
					new TemplateFileConfig { Glob = "*.css" }
				}
			};
			var prompts = new Dictionary<string, object>();

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts).ToList();

			//assert
			Assert.Equal(2, result.Count);
			Assert.Equal(globOneExpectedAmount, result[0].Files.Count());
			Assert.Equal(globTwoExpectedAmount, result[1].Files.Count());
		}

		[Fact]
		public void GivenATemplateWithOneGlob_AndOneMatchingVariableInThePrompt_WhenGetFilesToMoveIsCalled_ThenOneFileWillBeFoundToMove_WithTheCorrectVariable()
		{
			//arrange
			var expectedVariable = new KeyValuePair<string, object>("test", true);
			var prompts = new Dictionary<string, object> { { expectedVariable.Key, expectedVariable.Value } };

			var config = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
				{
					new TemplateFileConfig
					{
						Glob = "index.html",
						Variables = new List<string> { expectedVariable.Key }
					}
				}
			};

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.True(result.First().VariablesToApply.ContainsKey(expectedVariable.Key));
			Assert.Equal(expectedVariable.Value, result.First().VariablesToApply[expectedVariable.Key]);
		}

		[Fact]
		public void GivenATemplateWithOneGlob_AndNoMatchingVariableInThePrompt_WhenGetFilesToMoveIsCalled_ThenOneFileWillBeFoundToMove_WithNoVariables()
		{
			//arrange
			var prompts = new Dictionary<string, object> { { "variable", true } };

			var config = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
				{
					new TemplateFileConfig {
						Glob = "index.html",
						Variables = new List<string> { "test" } }
				}
			};

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.Empty(result.First().VariablesToApply);
		}

		[Fact]
		public void GivenATemplateWithOneGlob_AndSomeMatchingVariablesInThePrompt_WhenGetFilesToMoveIsCalled_ThenOneFileWillBeFoundToMove_WithOnlyTheVariablesWithAMatchingKey()
		{
			//arrange
			var expectedVariable = new KeyValuePair<string, object>("test", "mystring");
			var prompts = new Dictionary<string, object>
			{
				{ expectedVariable.Key, expectedVariable.Value },
				{ "variable", true }
			};

			var config = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
				{
					new TemplateFileConfig
					{
						Glob = "index.html",
						Variables = new List<string> { expectedVariable.Key }
					}
				}
			};

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts);

			//assert
			Assert.Single(result);
			Assert.True(result.First().VariablesToApply.ContainsKey(expectedVariable.Key));
			Assert.Equal(expectedVariable.Value, result.First().VariablesToApply[expectedVariable.Key]);
		}

		[Fact]
		public void GivenATemplateWithTwoGlobs_AndSomeMatchingVariablesInThePrompt_WhenGetFilesToMoveIsCalled_ThenTheResultWillOnlyContainTheVariablesWithAMatchingKey()
		{
			//arrange
			var expectedGlobOneVariable = new KeyValuePair<string, object>("globOneTest", "mystring");
			var expectedGlobTwoVariable = new KeyValuePair<string, object>("globTwoTest", 10);
			var prompts = new Dictionary<string, object>
			{
				{ expectedGlobOneVariable.Key, expectedGlobOneVariable.Value },
				{ expectedGlobTwoVariable.Key, expectedGlobTwoVariable.Value },
				{ "noneTest", true }
			};

			var config = new TemplateConfig
			{
				Files = new List<TemplateFileConfig>
				{
					new TemplateFileConfig
					{
						Glob = "*.html",
						Variables = new List<string> { expectedGlobOneVariable.Key }
					},
					new TemplateFileConfig
					{
						Glob = "*.css",
						Variables = new List<string> { expectedGlobTwoVariable.Key }
					}
				}
			};

			//act
			var result = FileProcessor.GetFilesToMove(TempPath, config, prompts).ToList();

			//assert
			Assert.Equal(2, result.Count);

			Assert.True(result[0].VariablesToApply.ContainsKey(expectedGlobOneVariable.Key));
			Assert.Equal(
				expectedGlobOneVariable.Value,
				result[0].VariablesToApply[expectedGlobOneVariable.Key]);

			Assert.True(result[1].VariablesToApply.ContainsKey(expectedGlobTwoVariable.Key));
			Assert.Equal(
				expectedGlobTwoVariable.Value,
				result[1].VariablesToApply[expectedGlobTwoVariable.Key]);
		}
	}
}
