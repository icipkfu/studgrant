using Grant.Core.Entities;
using Grant.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Services.DomainService.TelegramCommands
{
    class GetAccessCommand : ITelegramCommand
    {
        private IRepository<Settings> settingsRepo;
        private IRepository<TelegramUser> telegramUserRepo;

        public GetAccessCommand(IRepository<Settings> settingsRepo, IRepository<TelegramUser> telegramUserRepo)
        {
            this.settingsRepo = settingsRepo; // ! добавить хранение пароля в базе
            this.telegramUserRepo = telegramUserRepo;
        }

        public async Task<string> ExecuteCommandAsync(dynamic message)
        {
            string text = message["text"];
            string telegramId = message["from"]?.id;

            string password = "D5drqhQF";

            string passwordFromSender = text.Replace("getaccess ", "");

            if (passwordFromSender.Equals(password))
            {
                var telegramUser = await telegramUserRepo.GetAll()
                      .Include(x => x.Picture)
                      .Where(x => x.TelegramId == telegramId)
                      .FirstOrDefaultAsync();

                if(telegramUser != null)
                {
                    telegramUser.CanSendCommmand = true;
                    await telegramUserRepo.Update(telegramUser);
                    return "Доступ открыт.";
                }
                else
                {
                    return "Пользователь не найден.";
                }
            }
            else
            {
                return  "В доступе отказано.";
            }
        }
    }
}
