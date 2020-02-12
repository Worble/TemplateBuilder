namespace TemplateBuilder.Core.Tests.PromptReaderTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.Json;
	using FluentValidation;
	using TemplateBuilder.Core.Enums;
	using TemplateBuilder.Core.Models.Prompts;
	using Xunit;

	public class GetPromptsFromStringTests
	{
		#region Variables

		private const string MULTIPLE_VALID_PROMPTS = @"
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
		public void GivenAnEmptyString_WhenGetPromptsFromStringIsCalled_ThenAnArgumentExceptionIsThrown()
		{
			Assert.Throws<ArgumentException>(() => PromptReader.GetPromptsFromString(string.Empty));
		}

		[Fact]
		public void GivenAnJsonStringWithNoId_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAValidationException()
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

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithNoMessage_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAValidationException()
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

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithNoPromptType_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAJsonException()
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

			//act
			Assert.Throws<JsonException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithAnInvalidPromptType_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAJsonException()
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

			//act
			Assert.Throws<JsonException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithAPromptTypeOfZero_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAValidationException()
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

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithAPromptTypeHigherThenThree_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAInvalidPromptTypeException()
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

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithAPromptTypeOfInvalidType_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAValidationException()
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

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithMultipleValidPrompts_WhenGetPromptsFromStringIsCalled_ThenTheResultWillHaveTheCorrentAmountsOfPrompts()
		{
			//arrange
			const int expectedCount = 6;
			const int expectedStringCount = 2;
			const int expectedIntCount = 3;
			const string jsonString = MULTIPLE_VALID_PROMPTS;

			//act
			var actual = PromptReader.GetPromptsFromString(jsonString);

			//assert
			Assert.Equal(expectedCount, actual.Count());
			Assert.Single(actual.Where(e => e.PromptType == PromptType.Boolean));
			Assert.Equal(expectedStringCount, actual.Count(e => e.PromptType == PromptType.String));
			Assert.Equal(expectedIntCount, actual.Count(e => e.PromptType == PromptType.Int));
		}

		[Fact]
		public void test()
		{
			//arrange
			var expectedWhen = new PromptWhen
			{
				Id = "question",
				Is = "123"
			};
			var expectedPrompt = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = null,
				When = new List<PromptWhen>
				{
					expectedWhen
				}
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""when"": [{
			""id"": ""question"",
			""is"": ""123""
		}]
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

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
