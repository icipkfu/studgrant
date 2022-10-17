using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using Quartz;
using Quartz.Spi;

namespace Grant.Core.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceContainer _container;

        public JobFactory(IServiceContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_container.GetInstance(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
