using FluentValidation;

namespace Services.Features.Facebook.Models
{
    public class RunScraperRequestValidator : AbstractValidator<RunScraperRequest>
    {
        public RunScraperRequestValidator() 
        {
            RuleFor(x => x.PageUrl)
                .NotEmpty().WithMessage("A URL da página do Facebook é obrigatória.");
            RuleFor(x => x.MaxPosts)
                .GreaterThan(0).WithMessage("O número máximo de posts deve ser maior que zero.");
        }
    }
}
