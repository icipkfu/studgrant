namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Entities;
    using Core;
    using DataAccess;
    using Utils.Extensions;

    public class GrantAdminService : BaseDomainService<GrantAdmin>, IGrantAdminService
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public GrantAdminService(IRepository<GrantAdmin> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// Назначение администраторов гранта
        /// </summary>
        /// <param name="grantId">Идентификатор гранта</param>
        /// <param name="students">Список идентификаторов администраторов</param>
        /// /// <returns>Результат выполнения</returns>
        public async Task<DataResult> SetGrantAdmins(long grantId, long[] students)
        {
            //удаляем всех админов этого гранта

            var previous = await GetAll().Where(x => x.GrantId == grantId).ToListAsync();
            var previousStudIds = previous.Select(x => x.StudentId).ToDictionary(x => x);

            foreach (var rec in previous.Where(rec => !students.Contains(rec.StudentId)))
            {
                await Delete(rec.Id);
            }

            foreach (var studId in students)
            {
                if (!previousStudIds.ContainsKey(studId) && studId > 0)
                {
                    var newRec = new GrantAdmin
                    {
                        GrantId = grantId,
                        StudentId = studId
                    };

                    await Create(newRec);
                }

            }
    
            return DataResult.Ok();
        }

        /// <summary>
        /// Получение администраторов гранта
        /// </summary>
        /// <param name="grantId">Идентификатор гранта</param>
        /// <returns>Список идентификаторов администраторов</returns>
        public async Task<IEnumerable<Student>> GetGrantAdmins(long grantId)
        {
            return await  base.GetAll().Where(x => x.GrantId == grantId).Select(x => x.Student).ToListAsync();
        }
    }
}
