using FluentValidation;
using MediatR;

namespace Api.Behaviours
{
    /// <summary>
    /// Comportamento de pipeline do MediatR responsável por validar requisições usando FluentValidation.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição.</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta.</typeparam>
    /// <param name="validators">Coleção de validadores para o tipo de requisição.</param>
    public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        /// <summary>
        /// Executa a validação da requisição antes de passar para o próximo handler no pipeline.
        /// </summary>
        /// <param name="request">A requisição a ser validada.</param>
        /// <param name="next">Delegate para o próximo handler no pipeline.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resposta do handler subsequente.</returns>
        /// <exception cref="ValidationException">Lançada se houver falhas de validação.</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(result => result.Errors.Count > 0)
                    .SelectMany(result => result.Errors)
                    .ToList();

                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
