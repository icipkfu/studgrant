using Grant.Core.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Jobs
{
    public interface ICheckTelegramMsgJob : IJob
    {
    }

    public class CheckTelegramMsgJob : ICheckTelegramMsgJob
    {
        private readonly ITelegramService _telegramService;

        public CheckTelegramMsgJob(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public void Execute(IJobExecutionContext context)
        {
           // _telegramService.CheckTelegramMsgTask();
        }
    }
}
