using Grant.Core.Install;
using Grant.Core.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace Grant.Core
{
    public class AfterInstallInteceptor : IAfterInstallInterceptor
    {
        private readonly IJobFactory _jobFactory;

        public AfterInstallInteceptor(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public void Run()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.JobFactory = _jobFactory;
            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create(typeof(ISendNotificationsJob))
                .Build();

            // Trigger the job to run now, and then every 5 minutes
            ITrigger trigger = TriggerBuilder.Create()
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(60)
                  .RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);


            //--------------------------------------------
            
            IJobDetail job2 = JobBuilder.Create(typeof(IAutoBackupJob))
                .Build();
            
            var curTime = DateTime.Now;
            var launchTime = DateTime.Today;
            var seconds = (launchTime - curTime).TotalSeconds;
            
            ITrigger trigger2 = TriggerBuilder.Create()
                .StartNow()//StartAt(DateTime.Now.AddSeconds(seconds))
                .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
                .Build();
            
            sched.ScheduleJob(job2, trigger2);

            //--------------------------------------------

            IJobDetail job3 = JobBuilder.Create(typeof(ICheckTelegramMsgJob))
                .Build();

            ITrigger trigger3 = TriggerBuilder.Create()
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(5)
                  .RepeatForever())
              .Build();

            sched.ScheduleJob(job3, trigger3);

            //--------------------------------------------

        }
    }
}