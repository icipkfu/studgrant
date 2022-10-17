namespace Grant.Notifications
{
    using Core.Notification;
    using Core.Smtp;
    using Install;
    using Utils.Extensions;

    public class NotificationsInstaller : BaseInstaller
    {
        public override void Install()
        {
            this.Container.RegisterPerRequest<IMailSender, MailSender>();
            this.Container.RegisterPerRequest<INotificator, Notificator>();
            this.Container.RegisterTransient<INotificationQueueProvider, NotificationQueueProvider>();
        }
    }
}