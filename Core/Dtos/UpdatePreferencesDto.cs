namespace Core.Dtos
{
    /// <summary>
    /// DTO para atualização das preferências de notificação do usuário
    /// </summary>
    public class UpdatePreferencesDto
    {
        /// <summary>
        /// Lista de tópicos ou categorias que o usuário deseja receber notificações
        /// </summary>
        public string[] Topics { get; set; } = Array.Empty<string>();
    }
}
