namespace TemplateBuilder.Core.Models.Prompts
{
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Models.Prompts.Abstract;

	public class IntPrompt : AbstractPrompt
	{
		public int Value { get; set; }

		public static implicit operator IntPrompt(DeserializedTemplatePrompt template) =>
			new IntPrompt
			{
				Id = template.Id,
				Message = template.Message,
				Value = template.DefaultValue != null
					? template.DefaultValue is int value
						? value
						: int.Parse(template.DefaultValue.ToString())
					: default(int)
			};
	}
}
