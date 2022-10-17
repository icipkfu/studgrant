using System.ComponentModel.DataAnnotations.Schema;
using Grant.Core.Entities.Base;
using Grant.Core.Notification;

namespace Grant.Core.Entities
{
    /// <summary>
    /// Очередь уведомлений
    /// </summary>
    public class NotificationQueue : BaseEntity
    {
        public long StudentId { get; set; }

        public Student Student { get; set; }

        public long? GrantId { get; set; }

        public Grant Grant { get; set; }

        public string Email { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Parameters { get; set; }

        public bool Sent { get; set; }

        public string SendError { get; set; }
    }
}
