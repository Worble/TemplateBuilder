namespace TemplateBuilder.Core.Models
{
	using TemplateBuilder.Core.Enums;

	public class DeserializedTemplatePrompt
	{
		public PromptType PromptType { get; set; }
		public string Id { get; set; } = "";
		public string Message { get; set; } = "";
		public object? DefaultValue { get; set; }
	}
}
