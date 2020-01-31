namespace TemplateBuilder.Core.Tests.PromptReaderTests.BooleanPromptTests
{
	using System.Collections.Generic;
	using System.Linq;
	using TemplateBuilder.Core.Models.Prompts;
	using TemplateBuilder.Core.Models.Prompts.Abstract;
	using Xunit;

	public class GetPromptsFromString_StringPromptTests
	{
		[Fact]
		public void GivenAnJsonStringWithValidStringProperties_WhenGetPromptsFromStringIsCalled_ThenTheResultWillContainASingleValidStringPromptObject()
		{
			//arrange
			const int expectedCount = 1;
			var expectedObject = new StringPrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				Value = ""
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
			var expectedObject = new StringPrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				Value = "StringValue"
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
			var expectedObject = new StringPrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				Value = "123"
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
			var expectedObject = new StringPrompt
			{
				Id = "StringPromptId",
				Message = "String Prompt Message",
				Value = "False"
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

		private static void AssertStringPromptEquality(int expectedCount, StringPrompt expectedObject, IDictionary<string, AbstractPrompt> result)
		{
			Assert.Equal(expectedCount, result.Count);
			Assert.True(result.First().Key == expectedObject.Id);
			Assert.IsType<StringPrompt>(result.First().Value);
			var resultObject = (StringPrompt)result.First().Value;
			Assert.Equal(expectedObject.Id, resultObject.Id);
			Assert.Equal(expectedObject.Message, resultObject.Message);
			Assert.Equal(expectedObject.Value, resultObject.Value);
		}
	}
}
