using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Features.Auth.Models;
using Services.Features.Users.Repositories;
using Shared.Domain.Models;
using Shared.Infrastructure.Jwt;

namespace Services.Features.Auth.UseCases.Commands
{
    /// <summary>
    /// Handler responsável por processar a requisição de login de usuário, validando credenciais e gerando o token JWT.
    /// </summary>
    public class LoginHandler : IRequestHandler<LoginRequest, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginHandler> _logger;
        private readonly string _jwtKey = string.Empty;
        private readonly string _jwtIssuer = string.Empty;
        private readonly string _jwtAudience = string.Empty;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="LoginHandler"/>.
        /// </summary>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="configuration">Configuração da aplicação (JWT).</param>
        /// <param name="userRepository">Repositório de usuários.</param>
        public LoginHandler(ILogger<LoginHandler> logger, IConfiguration configuration, IUserRepository userRepository)
        {
            _logger = logger;
            _jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt key cannot be null.");
            _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "Issuer cannot be null.");
            _jwtAudience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "Audience cannot be null.");
            _userRepository = userRepository;
        }

        /// <summary>
        /// Processa a requisição de login, validando o usuário e senha, e retorna o resultado com token JWT.
        /// </summary>
        /// <param name="request">Requisição de login contendo email e senha.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado do login, incluindo token JWT em caso de sucesso.</returns>
        public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
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

                return Result<LoginResponse>.Fail(problemDetails);
            }

            // Verifica a senha usando BCrypt
            if (!VerifyPassword(request.Password, user.Password))
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Não autorizado",
                    Detail = "Credenciais inválidas.",
                    Status = 400
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<LoginResponse>.Fail(problemDetails);
            }

            // Gerar token JWT
            var token = JwtToken.GenerateJwtToken(_jwtKey, _jwtIssuer, _jwtAudience, request.Email, user.Name, user.UserId.ToString(), user.Role);

            _logger.LogInformation("Usuário {Email} autenticado com sucesso.", user.Email);

            return Result<LoginResponse>.Ok(new LoginResponse
            {
                Success = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                Message = "Login realizado com sucesso"
            });
        }

        /// <summary>
        /// Verifica se uma senha em texto plano corresponde ao hash armazenado.
        /// </summary>
        /// <param name="providedPassword">Senha em texto plano fornecida pelo usuário.</param>
        /// <param name="storedPassword">Hash da senha armazenado no banco de dados.</param>
        /// <returns>True se a senha corresponder ao hash, False caso contrário.</returns>
        private static bool VerifyPassword(string providedPassword, string storedPassword)
        {
            // BCrypt lida automaticamente com a comparação do hash
            return BCrypt.Net.BCrypt.Verify(providedPassword, storedPassword);
        }
    }
}
