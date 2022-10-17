﻿using Grant.Core.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Jobs
{
    public interface IAutoBackupJob : IJob
{

    }

    public class AutoBackupJob : IAutoBackupJob
    {
        private readonly ITaskService _taskService;

        public AutoBackupJob(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public void Execute(IJobExecutionContext context)
        {
            _taskService.AutoBackupTask();
        }
    }
}