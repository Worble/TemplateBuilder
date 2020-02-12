namespace TemplateBuilder.Core.Tests.PromptReaderTests.IntegerPromptTests
{
	using System.Collections.Generic;
	using System.Linq;
	using FluentValidation;
	using TemplateBuilder.Core.Models.Prompts;
	using Xunit;

	public class GetPromptsFromString_IntPromptTests
	{
		[Fact]
		public void GivenAnJsonStringWithValidIntProperties_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidIntPromptObject()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "IntegerPromptId",
				Message = "Integer Prompt Message",
				DefaultValue = null
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertIntPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidIntProperties_AndAIntDefaultValueOfOne_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidIntPromptObject_WithDefaultValueSetToOne()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "IntegerPromptId",
				Message = "Integer Prompt Message",
				DefaultValue = 1
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message"",
		""defaultValue"": 1
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertIntPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidIntProperties_AndAStringDefaultValueOfOne_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidIntPromptObject_WithDefaultValueSetToOne()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "IntegerPromptId",
				Message = "Integer Prompt Message",
				DefaultValue = 1
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message"",
		""defaultValue"": ""1""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertIntPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidIntProperties_AndAnInvalidStringDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAFormatException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message"",
		""defaultValue"": ""This is a test""
	}
]";

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithValidIntProperties_AndAnInvalidBooleanDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAFormatException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Int"",
		""id"": ""IntegerPromptId"",
		""message"": ""Integer Prompt Message"",
		""defaultValue"": true
	}
]";

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		private static void AssertIntPromptEquality(int expectedCount, TemplatePrompt expectedObject, IEnumerable<TemplatePrompt> result)
		{
			Assert.Equal(expectedCount, result.Count());
			Assert.True(result.First().Id == expectedObject.Id);
			var resultObject = result.First();
			Assert.Equal(expectedObject.Id, resultObject.Id);
			Assert.Equal(expectedObject.Message, resultObject.Message);
			Assert.Equal(expectedObject.DefaultValue, resultObject.DefaultValue == null ? (int?)null : resultObject.GetIntValue());
		}
	}
}
