namespace Services.Features.Auth.Models
{
    /// <summary>
    /// DTO para requisi��o de login.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email do usu�rio.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha do usu�rio.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisi��o de registro de usu�rio.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Nome do usu�rio.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Email do usu�rio.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Senha do usu�rio.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resultado de autentica��o.
    /// </summary>
    public class AuthResultDto
    {
        /// <summary>
        /// Indica se a autentica��o foi bem-sucedida.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Token JWT gerado.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Token de atualiza��o (refresh token).
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        /// <summary>
        /// Data de expira��o do token.
        /// </summary>
        public DateTime Expiration { get; set; }
        /// <summary>
        /// Mensagem de status ou erro.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
