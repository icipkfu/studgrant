using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Services
{
    public interface ITelegramService
    {
        Task CheckTelegramMsgTask();
        void sendSystemNotification(string text);
    }
}
