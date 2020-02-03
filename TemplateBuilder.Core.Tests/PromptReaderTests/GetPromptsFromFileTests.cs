namespace TemplateBuilder.Core.Tests.PromptReaderTests
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text.Json;
	using System.Threading.Tasks;
	using FluentValidation;
	using TemplateBuilder.Core.Exceptions;
	using TemplateBuilder.Core.Models.Prompts;
	using Xunit;

	public sealed class GetPromptsFromFileTests : IDisposable
	{
		private const string JSON_FILENAME = "templatePrompts.json";
		private static string TempFile => Path.Join(TempPath, JSON_FILENAME);
		private static string TempPath { get; } = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

		private const string VALID_MULTIPLE_PROMPTS = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	},
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message""
	},
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId2"",
		""message"": ""Integer Prompt Message""
	},
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId3"",
		""message"": ""Integer Prompt Message""
	},
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId"",
		""message"": ""String Prompt Message"",
		""defaultValue"": 123
	},
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId2"",
		""message"": ""String Prompt Message"",
		""defaultValue"": 123
	}
]";

		public GetPromptsFromFileTests()
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
		public async Task GivenAnEmptyString_WhenGetPromptsFromFileIsCalled_ThenAnArgumentExceptionIsThrown()
		{
			//arrange

			//act
			await Assert.ThrowsAsync<ArgumentException>(
				() => PromptReader.GetPromptsFromFile(string.Empty))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithNoId_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAValidationException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": """",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<ValidationException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithNoMessage_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAValidationException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": """"
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<ValidationException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithNoPromptType_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAJsonException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": """",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<JsonException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithAnInvalidPromptType_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAJsonException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Test"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<JsonException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithAPromptTypeOfZero_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAValidationException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""0"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<ValidationException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithAPromptTypeHigherThenThree_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAInvalidPromptTypeException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""4"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<InvalidPromptTypeException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithAPromptTypeOfInvalidType_WhenGetPromptsFromFileIsCalled_ThenTheResultWillThrowAValidationException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""INVALID_TYPE"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<ValidationException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenAJsonFileWithMultipleValidPrompts_WhenGetPromptsFromFileIsCalled_ThenTheResultWillHaveTheCorrentAmountsOfPrompts()
		{
			//arrange
			const int expectedCount = 6;
			const int expectedStringCount = 2;
			const int expectedIntCount = 3;
			const string jsonString = VALID_MULTIPLE_PROMPTS;
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			var actual = await PromptReader.GetPromptsFromFile(TempPath)
				.ConfigureAwait(false);

			//assert
			Assert.Equal(expectedCount, actual.Count);
			Assert.Single(actual.Where(e => e.Value.GetType() == typeof(BooleanPrompt)));
			Assert.Equal(expectedStringCount, actual.Count(e => e.Value.GetType() == typeof(StringPrompt)));
			Assert.Equal(expectedIntCount, actual.Count(e => e.Value.GetType() == typeof(IntPrompt)));
		}

		[Fact]
		public async Task GivenAJsonFileWithMultipleValidPrompts_AndADifferentFilename_WhenGetPromptsFromFileIsCalledWithTheFilename_ThenTheResultWillHaveTheCorrentAmountsOfPrompts()
		{
			//arrange
			const int expectedCount = 6;
			const int expectedStringCount = 2;
			const int expectedIntCount = 3;
			var filename = Guid.NewGuid().ToString();
			var file = Path.Join(TempPath, filename);
			const string jsonString = VALID_MULTIPLE_PROMPTS;
			await File.WriteAllTextAsync(file, jsonString).ConfigureAwait(false);

			//act
			var actual = await PromptReader.GetPromptsFromFile(TempPath, filename)
				.ConfigureAwait(false);

			//assert
			Assert.Equal(expectedCount, actual.Count);
			Assert.Single(actual.Where(e => e.Value.GetType() == typeof(BooleanPrompt)));
			Assert.Equal(expectedStringCount, actual.Count(e => e.Value.GetType() == typeof(StringPrompt)));
			Assert.Equal(expectedIntCount, actual.Count(e => e.Value.GetType() == typeof(IntPrompt)));
		}

		[Fact]
		public async Task GivenAJsonFileWithMultipleValidPrompts_AndADifferentFilename_WhenGetPromptsFromFileIsCalled_ThenTheResultWillHaveTheCorrentAmountsOfPrompts()
		{
			//arrange
			var filename = Guid.NewGuid().ToString();
			var file = Path.Join(TempPath, filename);
			const string jsonString = VALID_MULTIPLE_PROMPTS;
			await File.WriteAllTextAsync(file, jsonString).ConfigureAwait(false);

			//act
			await Assert.ThrowsAsync<FileNotFoundException>(
				() => PromptReader.GetPromptsFromFile(TempPath))
				.ConfigureAwait(false);
		}
	}
}
