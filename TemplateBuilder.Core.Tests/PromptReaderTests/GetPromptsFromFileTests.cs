namespace TemplateBuilder.Core.Tests.PromptReaderTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.Json;
	using System.Threading.Tasks;
	using FluentValidation;
	using TemplateBuilder.Core.Enums;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Tests.Abstract;
	using Xunit;

	public sealed class GetPromptsFromFileTests : FileTestBase
	{
		#region Variables

		internal override string Filename => "templatePrompts.json";

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

		#endregion Variables

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
			await Assert.ThrowsAsync<ValidationException>(
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
			Assert.Equal(expectedCount, actual.Count());
			Assert.Single(actual.Where(e => e.PromptType == PromptType.Boolean));
			Assert.Equal(expectedStringCount, actual.Count(e => e.PromptType == PromptType.String));
			Assert.Equal(expectedIntCount, actual.Count(e => e.PromptType == PromptType.Int));
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
			Assert.Equal(expectedCount, actual.Count());
			Assert.Single(actual.Where(e => e.PromptType == PromptType.Boolean));
			Assert.Equal(expectedStringCount, actual.Count(e => e.PromptType == PromptType.String));
			Assert.Equal(expectedIntCount, actual.Count(e => e.PromptType == PromptType.Int));
		}

		[Fact]
		public async Task GivenAJsonFileWithMultipleValidPrompts_AndADifferentFilename_WhenGetPromptsFromFileIsCalled_ThenAFileNotFoundExceptionWillBeThrown()
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

		[Theory]
		[InlineData(new object[] { "test", "\"test\"" })]
		[InlineData(new object[] { 123, "123" })]
		[InlineData(new object[] { true, "true" })]
		public async Task GivenAJsonStringWithAPromptWithAWhenArray_WhenGetPromptsFromFileIsCalled_ThenTheResultPromptWillHaveTheGivenWhenArray(object value, string jsonValue)
		{
			//arrange
			var expectedWhen = new PromptWhen
			{
				Id = "question",
				Is = value
			};
			var expectedPrompt = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = false,
				When = new List<PromptWhen>
				{
					expectedWhen
				}
			};
			var jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""when"": [{
			""id"": ""question"",
			""is"": " + jsonValue + @"
		}]
	}
]";
			await File.WriteAllTextAsync(TempFile, jsonString).ConfigureAwait(false);

			//act
			var result = await PromptReader.GetPromptsFromFile(TempPath).ConfigureAwait(false);

			//assert
			Assert.Single(result);
			var actualPrompt = result.First();
			Assert.Equal(expectedPrompt.When.Count, actualPrompt.When.Count);
			var actualWhen = actualPrompt.When[0];
			Assert.Equal(expectedWhen.Id, actualWhen.Id);
			Assert.Equal(expectedWhen.Is, actualWhen.Is);
		}
	}
}
