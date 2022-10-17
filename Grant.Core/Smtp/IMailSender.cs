namespace Grant.Core.Smtp
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    /// Интерфейс отправщика электронной почты
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// Адрес отправителя
        /// </summary>
        string From { get; }

        /// <summary>
        /// Отправить письмо
        /// </summary>
        Task Send(string to, string subject, string body, bool isHtml, Attachment[] attachments);
    }
}