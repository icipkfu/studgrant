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
    class UserSearchCommand : ITelegramCommand
    {
        private IRepository<Student> studentsRepo;

        public UserSearchCommand(IRepository<Student> studentsRepo)
        {
            this.studentsRepo = studentsRepo;
        }

        public async Task<string> ExecuteCommandAsync(dynamic message)
        {
            string text = message["text"];
            var phoneNumber = text.Replace("user ", "").Replace("+", "");

            if (phoneNumber.Length == 11) // есть "7" или "8" в начале
                phoneNumber = phoneNumber.Substring(1);


            int phone;
            //if (Int32.TryParse(phoneNumber, out phone))
            //{
                var result = await studentsRepo.GetAll()
                    .Where(x => x.Phone.Contains(phoneNumber))
                    .OrderBy(x => x.Name)
                    .ToArrayAsync();

                var count = result.Count();

                StringBuilder answer = new StringBuilder();
                answer.AppendLine($"Всего результатов: {count}");
                if (count > 0)
                {
                    answer.AppendLine($"_____________________");

                    List<Student> res = new List<Student>();

                    res = result.ToList();

                    foreach (var item in res)
                    {
                        answer.AppendLine($"Имя: {item.Name} {item.LastName} ");
                        answer.AppendLine($"Email: {item.Email} ");
                        answer.AppendLine($"Номер телефона: {item.Phone} ");
                        answer.AppendLine($"_____________________");

                    }
                }

                return answer.ToString();
            //}
            //else
            //{
            //    return "Не удалось обработать запрос. Возможно была допущена ошибка при наборе номера телефона. \n Пример команды: user 9991234567";
            //}

        }
    }
}
