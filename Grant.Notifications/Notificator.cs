using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Grant.Notifications
{
    using System;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Core;
    using Core.Logging;
    using Core.Notification;
    using Core.Smtp;
    using Utils;
    using Utils.Extensions;

    public class Notificator : INotificator
    {
        private readonly IMailSender _sender;
        private readonly ITemplateResolver _templateResolver;
        private readonly ILogManager _log;

        /// <summary>
        /// 
        /// </summary>
        public Notificator(IMailSender sender, ITemplateResolver templateResolver, ILogManager log)
        {
            _sender = sender;
            _templateResolver = templateResolver;
            _log = log;
        }

        public async Task<DataResult> NotifyUser(string to, NotificationType type, Dictionary<string, object> data)
        {
            ArgumentChecker.NotEmpty(to, "to");

            NotificationTemplate template;
            try
            {
                template = _templateResolver.GetTemplate(type);
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Error getting template {0}", type), e);
                return DataResult.Failure(e.Message);
            }

            string html;
            using (var sr = new StreamReader(template.Template))
            {
                html = sr.ReadToEnd();
            }

            if (data != null)
            {
                foreach (var prop in data)
                {
                    if (prop.Value != null)
                    {
                        var str = "{{" + prop.Key + "}}";

                        html = html.Replace(str, prop.Value.ToString());
                    }
                }
            }
            
            var regex = new Regex(@"\{\{.*\}\}");

            html = regex.Replace(html, "");
            var displayAttribute = type.GetAttribute<DisplayAttribute>();
            var title = displayAttribute != null ? displayAttribute.Name : type.ToString();

            var attachments = new List<Attachment>();
            if (!template.Attachments.IsEmpty())
            {
                foreach (var attach in template.Attachments)
                {
                    var file = new Attachment(attach.Stream, attach.Name, "image/png")
                    {
                        ContentId = attach.Name,
                        ContentDisposition =
                        {
                            Inline = true,
                            DispositionType = DispositionTypeNames.Inline
                        }
                    };

                    attachments.Add(file);
                }
            }

            try
            {
                await _sender.Send(to, title, html, true, attachments.ToArray());
            }
            catch (Exception e)
            {
                _log.Error(string.Format("Error sending email to {0}", to), e);
                return DataResult.Failure(e.Message);
            }

            return DataResult.Ok();
        }
    }
}