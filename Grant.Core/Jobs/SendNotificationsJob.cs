using Grant.Core.Notification;
using Quartz;

namespace Grant.Core.Jobs
{
    public interface ISendNotificationsJob : IJob
    {
    }

    public class SendNotificationsJob : ISendNotificationsJob
    {
        private readonly INotificationQueueProvider _notificationQueueProvider;

        public SendNotificationsJob(INotificationQueueProvider notificationQueueProvider)
        {
            _notificationQueueProvider = notificationQueueProvider;
        }

        public void Execute(IJobExecutionContext context)
        {
            _notificationQueueProvider.SentQueue();
        }
    }
}
