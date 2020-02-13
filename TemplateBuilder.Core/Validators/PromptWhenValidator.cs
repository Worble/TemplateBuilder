namespace TemplateBuilder.Core.Validators
{
	using FluentValidation;
	using TemplateBuilder.Core.Models.Prompts;

	public class PromptWhenValidator : AbstractValidator<PromptWhen>
	{
		public PromptWhenValidator()
		{
			RuleFor(x => x.Id).NotEmpty();
			RuleFor(x => x.Is).NotEmpty();
		}
	}
}
