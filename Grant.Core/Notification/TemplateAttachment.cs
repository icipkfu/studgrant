namespace Grant.Core.Notification
{
    using System.IO;

    public class TemplateAttachment
    {
        public Stream Stream { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }
    }
}