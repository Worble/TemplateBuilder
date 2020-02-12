namespace TemplateBuilder.Core.Tests.PromptReaderTests.BooleanPromptTests
{
	using System.Collections.Generic;
	using System.Linq;
	using FluentValidation;
	using TemplateBuilder.Core.Models.Prompts;
	using Xunit;

	public class GetPromptsFromString_BooleanPromptTests
	{
		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidBooleanPromptObject()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = null
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertBooleanPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndAStringDefaultValueOfTrue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidBooleanPromptObject_WithDefaultValueSetToTrue()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = true
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": ""true""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertBooleanPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndADefaultValueOfTrue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidBooleanPromptObject_WithDefaultValueSetToTrue()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = true
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": true
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertBooleanPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndAStringDefaultValueOfFalse_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidBooleanPromptObject_WithDefaultValueSetToFalse()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = false
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": ""false""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertBooleanPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndADefaultValueOfFalse_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidBooleanPromptObject_WithDefaultValueSetToFalse()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "BooleanPromptId",
				Message = "Boolean Prompt Message",
				DefaultValue = false
			};
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": false
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertBooleanPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndAnInvalidStringDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAFormatException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": ""This is a test""
	}
]";

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		[Fact]
		public void GivenAnJsonStringWithValidBooleanProperties_AndAnInvalidNumberDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillThrowAFormatException()
		{
			//arrange
			const string jsonString = @"
[
	{
		""promptType"": ""Boolean"",
		""id"": ""BooleanPromptId"",
		""message"": ""Boolean Prompt Message"",
		""defaultValue"": 123
	}
]";

			//act
			Assert.Throws<ValidationException>(() => PromptReader.GetPromptsFromString(jsonString));
		}

		private static void AssertBooleanPromptEquality(int expectedCount, TemplatePrompt expectedObject, IEnumerable<TemplatePrompt> result)
		{
			Assert.Equal(expectedCount, result.Count());
			Assert.True(result.First().Id == expectedObject.Id);
			var resultObject = result.First();
			Assert.Equal(expectedObject.Id, resultObject.Id);
			Assert.Equal(expectedObject.Message, resultObject.Message);
			Assert.Equal(expectedObject.DefaultValue, resultObject.DefaultValue == null ? (bool?)null : resultObject.GetBoolValue());
		}
	}
}
