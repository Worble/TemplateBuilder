namespace TemplateBuilder.Core.Models.Prompts
{
	using System;
	using System.Collections.Generic;
	using TemplateBuilder.Core.Enums;

	public class TemplatePrompt
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
		protected internal object? _defaultValue;

		public PromptType PromptType { get; set; }
		public string Id { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public List<PromptWhen> When { get; set; } = new List<PromptWhen>();

		public object DefaultValue
		{
			get => GetValue();
			set => _defaultValue = value;
		}

		private object GetValue()
		{
			return PromptType switch
			{
				PromptType.Boolean => GetBoolValue(),
				PromptType.String => GetStringValue(),
				PromptType.Int => GetIntValue(),
				_ => throw new IndexOutOfRangeException(),
			};
		}

		private string GetStringValue()
		{
			if (_defaultValue == null)
			{
				return string.Empty;
			}
			return _defaultValue.ToString();
		}

		private bool GetBoolValue()
		{
			if (_defaultValue == null)
			{
				return default;
			}
			if (_defaultValue is bool boolValue)
			{
				return boolValue;
			}
			else if (bool.TryParse(_defaultValue.ToString(), out boolValue))
			{
				return boolValue;
			}
			return default;
		}

		private int GetIntValue()
		{
			if (_defaultValue == null)
			{
				return default;
			}
			if (_defaultValue is int intValue)
			{
				return intValue;
			}
			else if (int.TryParse(_defaultValue.ToString(), out intValue))
			{
				return intValue;
			}
			return default;
		}
	}
}
