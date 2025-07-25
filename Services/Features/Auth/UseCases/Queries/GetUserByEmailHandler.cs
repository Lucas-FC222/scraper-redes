using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Features.Auth.Models;
using Services.Features.Users.Repositories;
using Shared.Domain.Models;
using System.Text.Json;

namespace Services.Features.Auth.UseCases.Queries
{
    /// <summary>
    /// Handler responsável por processar a requisição de consulta de usuário por e-mail.
    /// </summary>
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailRequest, Result<GetUserByEmailResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByEmailHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GetUserByEmailHandler"/>.
        /// </summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        public GetUserByEmailHandler(IUserRepository userRepository, ILogger<GetUserByEmailHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa a requisição para obter um usuário pelo e-mail, retornando os dados do usuário ou erro caso não encontrado.
        /// </summary>
        /// <param name="request">Requisição contendo o e-mail do usuário.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado da consulta, contendo os dados do usuário ou detalhes do erro.</returns>
        public async Task<Result<GetUserByEmailResponse>> Handle(GetUserByEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = "Nenhum usuário encontrado com o email fornecido.",
                    Status = 404
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<GetUserByEmailResponse>.Fail(problemDetails);
            }

            return Result<GetUserByEmailResponse>.Ok(JsonSerializer.Deserialize<GetUserByEmailResponse>(JsonSerializer.Serialize(user))!);
        }
    }
}
