namespace Services.Features.Auth.Models
{
    /// <summary>
    /// DTO para requisição de login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha do usuário.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de registro de usuário.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Email do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha do usuário.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resultado de autenticação.
    /// </summary>
    public class AuthResultDto
    {
        /// <summary>
        /// Indica se a autenticação foi bem-sucedida.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Token JWT gerado.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Token de atualização (refresh token).
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        /// <summary>
        /// Data de expiração do token.
        /// </summary>
        public DateTime Expiration { get; set; }
        /// <summary>
        /// Mensagem de status ou erro.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
