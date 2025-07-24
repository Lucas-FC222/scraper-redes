namespace Core.Services
{
    public interface INotificationProcessorService
    {
        /// <summary>
        /// Roda um ciclo de notificações: para cada usuário, verifica novos posts e marca notificações.
        /// </summary>
        Task RunAsync(CancellationToken cancellationToken);
    }
}
