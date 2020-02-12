namespace TemplateBuilder.Core.Models.Prompts
{
	using System.Collections.Generic;
	using TemplateBuilder.Core.Enums;

	public class TemplatePrompt
	{
		public PromptType PromptType { get; set; }
		public string Id { get; set; } = "";
		public string Message { get; set; } = "";
		public object? DefaultValue { get; set; }
		public List<PromptWhen> When { get; set; } = new List<PromptWhen>();

		public string GetStringValue()
		{
			var value = string.Empty;
			if (DefaultValue != null)
			{
				value = DefaultValue.ToString();
			}

			return value;
		}

		public bool GetBoolValue()
		{
			var value = false;
			if (DefaultValue != null)
			{
				if (DefaultValue is bool boolValue)
				{
					value = boolValue;
				}
				else if (bool.TryParse(DefaultValue.ToString(), out boolValue))
				{
					value = boolValue;
				}
			}

			return value;
		}

		public int GetIntValue()
		{
			int value = default;
			if (DefaultValue != null)
			{
				if (DefaultValue is int intValue)
				{
					value = intValue;
				}
				else if (int.TryParse(DefaultValue.ToString(), out intValue))
				{
					value = intValue;
				}
			}

			return value;
		}
	}
}
