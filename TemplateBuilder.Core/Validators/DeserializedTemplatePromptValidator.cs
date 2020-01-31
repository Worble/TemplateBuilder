namespace TemplateBuilder.Core.Validators
{
	using FluentValidation;
	using TemplateBuilder.Core.Models;

	public class DeserializedTemplatePromptValidator : AbstractValidator<DeserializedTemplatePrompt>
	{
		public DeserializedTemplatePromptValidator()
		{
			RuleFor(x => x.PromptType).NotEmpty();
			RuleFor(x => x.Id).NotEmpty();
			RuleFor(x => x.Message).NotEmpty();
		}
	}
}
