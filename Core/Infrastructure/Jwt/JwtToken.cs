using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shared.Infrastructure.Jwt
{
    /// <summary>
    /// Classe utilitária para geração de tokens JWT para autenticação e autorização.
    /// </summary>
    public static class JwtToken
    {
        /// <summary>
        /// Gera um token JWT com as informações do usuário e configurações fornecidas.
        /// </summary>
        /// <param name="jwtKey">Chave secreta para assinatura do token.</param>
        /// <param name="jwtIssuer">Emissor do token (Issuer).</param>
        /// <param name="jwtAudience">Audiência do token (Audience).</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="name">Nome do usuário.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="userRole">Papel (role) do usuário.</param>
        /// <returns>Token JWT gerado como string.</returns>
        public static string GenerateJwtToken(string jwtKey, string jwtIssuer, string jwtAudience, string email, string name, string userId, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, email),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Name, name),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
