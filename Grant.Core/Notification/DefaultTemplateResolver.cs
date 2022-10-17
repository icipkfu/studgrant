namespace Grant.Core.Notification
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Context;
    using Logging;
    using Utils.Extensions;

    //TODO: если будет тормозить, то сделать чтоб шаблон и аттачи один раз читались с диска

    public class DefaultTemplateResolver : ITemplateResolver
    {
        private readonly ILogManager _log;

        private readonly string _pathToTemplates;

        public DefaultTemplateResolver(ILogManager log)
        {
            _log = log;

            _pathToTemplates = ApplicationContext.Current.MapPath("notificationTemplates");

            if (!Directory.Exists(_pathToTemplates))
            {
                _log.Error("Notification template directory not exists");
                Directory.CreateDirectory(_pathToTemplates);
            }
        }

        public NotificationTemplate GetTemplate(NotificationType type)
        {
            var str = type.ToString();

            var dir = Path.Combine(_pathToTemplates, str);

            if (!Directory.Exists(dir))
            {
                _log.Error(string.Format("Directory for notification template {0} not exists", str));
                Directory.CreateDirectory(dir);
            }

            var template = GetTemplate(dir);

            var attachs = GetAttachments(dir);

            return new NotificationTemplate(template, attachs);
        }

        private static MemoryStream GetTemplate(string dir)
        {
            var path = Path.Combine(dir, "index.html");

            var bytes = File.ReadAllBytes(path);

            return new MemoryStream(bytes);
        }

        private static List<TemplateAttachment> GetAttachments(string dir)
        {
            var directory = new DirectoryInfo(dir);

            var files = directory.GetFiles();

            var result = new List<TemplateAttachment>();

            foreach (var file in files.Where(x => x.Name != "index.html"))
            {
                result.Add(new TemplateAttachment
                {
                    Stream = new MemoryStream(File.ReadAllBytes(file.FullName)),
                    Name = file.Name,
                    Extension = file.Extension
                });
            }

            return result;
        }
    }
}