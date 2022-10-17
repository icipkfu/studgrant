using Grant.Core.Install;
using Grant.Core.Jobs;
using Quartz.Spi;

namespace Grant.Core
{
    using Grant.Install;
    using Grant.Utils.Extensions;
    using Notification;
    using Services;
    using Services.Impl;

    public class CoreInstaller : BaseInstaller
    {
        public override void Install()
        {
            this.Container.RegisterTransient<IDateTimeProvider, TimeProvider>();

            this.Container.RegisterSingleton<ITemplateResolver, DefaultTemplateResolver>();

            this.Container.RegisterTransient<IAfterInstallInterceptor, AfterInstallInteceptor>();

            this.Container.RegisterTransient<IJobFactory, JobFactory>();

            this.Container.RegisterTransient<ISendNotificationsJob, SendNotificationsJob>();

            this.Container.RegisterTransient<IAutoBackupJob, AutoBackupJob>();

            this.Container.RegisterTransient<ITaskService, TaskService>();

            this.Container.RegisterTransient<ICheckTelegramMsgJob, CheckTelegramMsgJob>();

        }
    }
}