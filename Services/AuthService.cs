using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Dtos;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica um usuário com base no email e senha
        /// </summary>
        /// <param name="loginDto">DTO contendo email e senha</param>
        /// <returns>Resultado da autenticação com token JWT se bem-sucedido</returns>
        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            
            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            // Verifica a senha usando BCrypt
            if (!VerifyPassword(loginDto.Password, user.Password))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Credenciais inválidas"
                };
            }

            // Gerar token JWT
            var token = GenerateJwtToken(user);
            
            return new AuthResultDto
            {
                Success = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                Message = "Login realizado com sucesso"
            };
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registerDto">DTO contendo dados para registro (nome, email, senha)</param>
        /// <returns>Resultado do registro com token JWT se bem-sucedido</returns>
        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            // Verificar se o email já existe
            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Este email já está em uso"
                };
            }

            // Criar novo usuário
            var user = new AppUser
            {
                UserId = Guid.NewGuid(),
                Email = registerDto.Email,
                Name = registerDto.Name,
                Password = HashPassword(registerDto.Password), // Hash com BCrypt
                Role = "User" // Papel padrão
            };

            // Salvar usuário no banco
            await _userRepository.CreateAsync(user);

            // Gerar token JWT
            var token = GenerateJwtToken(user);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                Message = "Registro realizado com sucesso"
            };
        }

        /// <summary>
        /// Recupera um usuário pelo email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Objeto AppUser se encontrado, null caso contrário</returns>
        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Gera um token JWT para um usuário
        /// </summary>
        /// <param name="user">Objeto AppUser para o qual gerar o token</param>
        /// <returns>Token JWT em formato string</returns>
        public string GenerateJwtToken(AppUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "defaultsecretkey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Gera um hash seguro da senha usando BCrypt
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash seguro da senha</returns>
        private string HashPassword(string password)
        {
            // Usando BCrypt para gerar um hash seguro da senha
            // WorkFactor 12 oferece um bom equilíbrio entre segurança e performance
            // Maior workFactor = mais seguro, mas mais lento
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Verifica se uma senha em texto plano corresponde ao hash armazenado
        /// </summary>
        /// <param name="providedPassword">Senha em texto plano fornecida pelo usuário</param>
        /// <param name="storedPassword">Hash da senha armazenado no banco de dados</param>
        /// <returns>True se a senha corresponder ao hash, False caso contrário</returns>
        private bool VerifyPassword(string providedPassword, string storedPassword)
        {
            // BCrypt lida automaticamente com a comparação do hash
            return BCrypt.Net.BCrypt.Verify(providedPassword, storedPassword);
        }
    }
}
