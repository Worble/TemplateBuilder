namespace TemplateBuilder.Core.Models.Prompts.Abstract
{
	public abstract class AbstractPrompt
	{
		public string Id { get; set; } = string.Empty;

		public string Message { get; set; } = string.Empty;
	}
}
