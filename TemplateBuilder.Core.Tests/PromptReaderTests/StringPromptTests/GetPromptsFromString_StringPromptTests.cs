namespace TemplateBuilder.Core.Tests.PromptReaderTests.StringPromptTests
{
	using System.Collections.Generic;
	using System.Linq;
	using TemplateBuilder.Core.Models.Prompts;
	using Xunit;

	public class GetPromptsFromString_StringPromptTests
	{
		[Fact]
		public void GivenAnJsonStringWithValidStringProperties_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidStringPromptObject()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				DefaultValue = null
			};
			const string jsonString = @"
[
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId"",
		""message"": ""String Prompt Message""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertStringPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidStringProperties_AndAStringDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleStringPromptObject_WithADefaultValueSet()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				DefaultValue = "StringValue"
			};
			const string jsonString = @"
[
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId"",
		""message"": ""String Prompt Message"",
		""defaultValue"": ""StringValue""
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertStringPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidStringProperties_AndAnIntDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleStringPromptObject_WithTheIntDefaultValueSet()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				DefaultValue = "123"
			};
			const string jsonString = @"
[
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId"",
		""message"": ""String Prompt Message"",
		""defaultValue"": 123
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertStringPromptEquality(expectedCount, expectedObject, result);
		}

		[Fact]
		public void GivenAnJsonStringWithValidStringProperties_AndABooleanDefaultValue_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleStringPromptObject_WithTheBooleanDefaultValueSet()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new TemplatePrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				DefaultValue = "False"
			};
			const string jsonString = @"
[
	{
		""promptType"": ""String"",
		""id"": ""StringPromptId"",
		""message"": ""String Prompt Message"",
		""defaultValue"": false
	}
]";

			//act
			var result = PromptReader.GetPromptsFromString(jsonString);

			//assert
			AssertStringPromptEquality(expectedCount, expectedObject, result);
		}

		private static void AssertStringPromptEquality(int expectedCount, TemplatePrompt expectedObject, IEnumerable<TemplatePrompt> result)
		{
			Assert.Equal(expectedCount, result.Count());
			Assert.True(result.First().Id == expectedObject.Id);
			var resultObject = result.First();
			Assert.Equal(expectedObject.Id, resultObject.Id);
			Assert.Equal(expectedObject.Message, resultObject.Message);
			Assert.Equal(expectedObject.DefaultValue, resultObject.DefaultValue == null ? null : resultObject.GetStringValue());
		}
	}
}
