namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Resposta da consulta de usuário por e-mail, contendo dados do usuário encontrado.
    /// </summary>
    public class GetUserByEmailResponse
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Senha do usuário (armazenada de forma segura).
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// Papel do usuário na aplicação ("User" ou "Admin").
        /// </summary>
        public string Role { get; set; } = "User"; // "User" ou "Admin"

        /// <summary>
        /// Lista de tópicos de interesse do usuário.
        /// </summary>
        public ICollection<string> TopicPreferences { get; set; }
            = new List<string>();
    }
}
