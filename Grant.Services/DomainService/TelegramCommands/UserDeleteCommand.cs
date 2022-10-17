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
    class UserDeleteCommand : ITelegramCommand
    {
        private IRepository<Student> studentsRepo;

        public UserDeleteCommand(IRepository<Student> studentsRepo)
        {
            this.studentsRepo = studentsRepo;
        }

        public async Task<string> ExecuteCommandAsync(dynamic message)
        {
            string text = message["text"];
            var email = text.Replace("deleteuser ", "");

            var result = await studentsRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.Email.Equals(email));

            if (result != null)
            {
                if(result.DeletedMark == false)
                {
                    result.DeletedMark = true;
                    await studentsRepo.Update(result);
                    return $"Пользователь {result.Name} {result.LastName} ({result.Email}) успешно удален.";
                }
                else
                {
                    return $"Пользователь {result.Name} {result.LastName} ({result.Email}) уже был удален ранее.";
                }
            }
            else
            {
                return $"Пользователь c email {email} не найден.";
            }

        }
    }
}
