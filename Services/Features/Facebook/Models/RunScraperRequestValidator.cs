using FluentValidation;

namespace Services.Features.Facebook.Models
{
    /// <summary>
    /// Validador para a requisição <see cref="RunScraperRequest"/>, garantindo que os dados estejam corretos antes da execução do scraper.
    /// </summary>
    public class RunScraperRequestValidator : AbstractValidator<RunScraperRequest>
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="RunScraperRequestValidator"/> e define as regras de validação.
        /// </summary>
        public RunScraperRequestValidator() 
        {
            RuleFor(x => x.PageUrl)
                .NotEmpty().WithMessage("A URL da página do Facebook é obrigatória.");
            RuleFor(x => x.MaxPosts)
                .GreaterThan(0).WithMessage("O número máximo de posts deve ser maior que zero.");
        }
    }
}
