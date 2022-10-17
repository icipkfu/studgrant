using System;

namespace Grant.Notifications
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Core.Config;
    using Core.Logging;
    using Core.Smtp;
    using Utils;

    public class MailSender : IMailSender
    {
        private readonly SmtpClient _client;
        private readonly ILogManager _logManager;
        public string From { get; private set; }

        /// <summary>
        /// .ctor
        /// </summary>
        public MailSender(IConfigProvider configProvider, ILogManager logManager)
        {
            _logManager = logManager;

            var config = configProvider.GetModuleConfig("Smtp");
            var username = config.GetAs<string>("User");
            var password = config.GetAs<string>("Password");

            var host = config.GetAs<string>("Host");
            var port = config.GetAs<int>("Port");
            var enableSsl = config.GetAs<bool>("EnableSsl");
            From = config.GetAs<string>("From");

            // ReSharper disable once UseObjectOrCollectionInitializer
            // do not use object initializer
            _client = new SmtpClient(host, port);

            _client.EnableSsl = enableSsl;
           // _client.UseDefaultCredentials = false;
           // _client.DeliveryMethod = SmtpDeliveryMethod.Network;
           // _client.DeliveryFormat = SmtpDeliveryFormat.SevenBit;
            _client.Credentials = new NetworkCredential(username, password);
        }

        /// <summary>
        /// Отправить письмо
        /// </summary>
        public async Task Send(string to, string subject, string body, bool isHtml, Attachment[] attachments)
        {
            ArgumentChecker.NotEmpty(to, "to");
            ArgumentChecker.NotEmpty(body, "body");

            var message = new MailMessage(From, to)
            {
                Sender = new MailAddress(From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var attach in attachments)
            {
                message.Attachments.Add(attach);
            }

            _logManager.Info(string.Format("Sending email message from:{0};to:{1};subject:{2}", From, to, subject));

            try
            {
#if DEBUG
                // await _client.SendMailAsync(message);
#else
                await _client.SendMailAsync(message);
#endif
            }
            catch (Exception ex)
            {
                var t = "test";
            }
            
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}