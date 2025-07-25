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
    /// Handler responsável por processar a requisição de registro de novo usuário, criando o usuário e gerando o token JWT.
    /// </summary>
    public class RegisterHandler : IRequestHandler<RegisterRequest, Result<RegisterResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterHandler> _logger;
        private readonly string _jwtKey = string.Empty;
        private readonly string _jwtIssuer = string.Empty;
        private readonly string _jwtAudience = string.Empty;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RegisterHandler"/>.
        /// </summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="logger">Logger para registro de eventos.</param>
        /// <param name="configuration">Configuração da aplicação (JWT).</param>
        public RegisterHandler(IUserRepository userRepository, ILogger<RegisterHandler> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt key cannot be null.");
            _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "Issuer cannot be null.");
            _jwtAudience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "Audience cannot be null.");
        }

        /// <summary>
        /// Processa a requisição de registro, criando o usuário e retornando o resultado com token JWT.
        /// </summary>
        /// <param name="request">Requisição de registro contendo nome, email e senha.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Resultado do registro, incluindo token JWT em caso de sucesso.</returns>
        public async Task<Result<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var userFound = await _userRepository.ExistsByEmailAsync(request.Email);

            // Verificar se o email já existe
            if (userFound)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Email já cadastrado",
                    Detail = "Este email já está em uso. Por favor, escolha outro.",
                    Status = 400 // Bad Request
                };

                _logger.LogWarning(problemDetails.Detail, problemDetails.Status);

                return Result<RegisterResponse>.Fail(problemDetails);
            }

            // Criar novo usuário
            var user = new AppUser
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                Password = HashPassword(request.Password), // Hash com BCrypt
                Role = "User" // Papel padrão
            };

            // Salvar usuário no banco
            await _userRepository.CreateAsync(user);

            // Gerar token JWT
            var token = JwtToken.GenerateJwtToken(_jwtKey, _jwtIssuer, _jwtAudience, request.Email, user.Name, user.UserId.ToString(), user.Role);

            return Result<RegisterResponse>.Ok(new RegisterResponse
            {
                Success = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                Message = "Registro realizado com sucesso"
            });
        }

        /// <summary>
        /// Gera um hash seguro da senha usando BCrypt.
        /// </summary>
        /// <param name="password">Senha em texto plano.</param>
        /// <returns>Hash seguro da senha.</returns>
        private static string HashPassword(string password)
        {
            // Usando BCrypt para gerar um hash seguro da senha
            // WorkFactor 12 oferece um bom equilíbrio entre segurança e performance
            // Maior workFactor = mais seguro, mas mais lento
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
    }
}
