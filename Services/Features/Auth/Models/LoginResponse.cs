namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Resposta da autenticação de usuário (login), contendo tokens e informações de status.
    /// </summary>
    public class LoginResponse
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
