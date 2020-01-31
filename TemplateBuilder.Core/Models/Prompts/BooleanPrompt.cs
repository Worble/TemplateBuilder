namespace TemplateBuilder.Core.Models.Prompts
{
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	public class BooleanPrompt : AbstractPrompt
	{
		public bool Value { get; set; }

		public static implicit operator BooleanPrompt(DeserializedTemplatePrompt template) =>
			new BooleanPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue != null
					? template.DefaultValue is bool value
						? value
						: bool.Parse(template.DefaultValue.ToString())
					: default(bool)
			};
	}
}
