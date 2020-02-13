namespace TemplateBuilder.Core.Validators
{
	using FluentValidation;
	using TemplateBuilder.Core.Enums;
	using TemplateBuilder.Core.Models.Prompts;

	public class TemplatePromptValidator : AbstractValidator<TemplatePrompt>
	{
		public TemplatePromptValidator()
		{
			RuleFor(x => x.PromptType).NotEmpty().IsInEnum();
			RuleFor(x => x.Id).NotEmpty();
			RuleFor(x => x.Message).NotEmpty();
			RuleFor(x => x._defaultValue)
				.Must((prompt, value) => TryCast(prompt.PromptType, value!))
				.When(x => x._defaultValue != null)
				.WithMessage("Default Value is not the correct type for prompt");
			RuleForEach(x => x.When).SetValidator(new PromptWhenValidator());
		}

		private bool TryCast(PromptType promptType, object value)
		{
			return promptType switch
			{
				PromptType.Boolean => value is bool || bool.TryParse(value.ToString(), out var _),
				PromptType.Int => value is int || int.TryParse(value.ToString(), out var _),
				PromptType.String => true,
				_ => false,
			};
		}
	}
}
