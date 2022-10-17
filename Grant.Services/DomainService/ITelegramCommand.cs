using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Services.DomainService
{
    public interface ITelegramCommand
    {
        Task<string> ExecuteCommandAsync(dynamic message);
    }
}
