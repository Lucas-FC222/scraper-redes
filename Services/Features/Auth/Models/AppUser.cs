namespace Services.Features.Auth.Models
{
    /// <summary>
    /// Representa um usuário da aplicação, contendo informações de autenticação, perfil e preferências.
    /// </summary>
    public class AppUser
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// E-mail do usuário (usado para login).
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
