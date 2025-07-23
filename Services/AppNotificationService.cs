//using Core;
//using Infra;

//namespace Services
//{

//    public class AppNotificationService : INotificationService
//    {
//        private readonly INotificationRepository _repo;

//        public AppNotificationService(INotificationRepository repo)
//        {
//            _repo = repo;
//        }

//        /// <summary>
//        /// Se ainda não notificou esse post para o usuário, marca em SentNotifications.
//        /// A UI lê InstagramPosts JOIN SentNotifications para exibir.
//        /// </summary>
//        public async Task SendAsync(AppUser user, Core.InstagramPost post)
//        {
//            if (await _repo.WasNotifiedAsync(user.UserId, post.Id))
//                return;

//            await _repo.MarkNotifiedAsync(user.UserId, post.Id);
//        }
//    }
//}
