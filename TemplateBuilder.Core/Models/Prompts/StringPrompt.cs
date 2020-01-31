namespace TemplateBuilder.Core.Models.Prompts
{
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	public class StringPrompt : AbstractPrompt
	{
		public string Value { get; set; } = string.Empty;

		public static implicit operator StringPrompt(DeserializedTemplatePrompt template) =>
			new StringPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue?.ToString() ?? string.Empty
			};
	}
}
