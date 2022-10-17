namespace Grant.Core.Notification
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Utils;
    using Utils.Extensions;

    public class NotificationTemplate
    {
        public NotificationTemplate(Stream template, IEnumerable<TemplateAttachment> attachments)
        {
            ArgumentChecker.NotNull(template, "template");

            this.Template = template;
            if (!attachments.IsEmpty())
            {
                this.Attachments = attachments.ToArray();
            }
        }

        /// <summary>
        /// Вложения
        /// </summary>
        public TemplateAttachment[] Attachments { get; set; }

        /// <summary>
        /// Шаблон
        /// </summary>
        public Stream Template { get; set; }
    }
}