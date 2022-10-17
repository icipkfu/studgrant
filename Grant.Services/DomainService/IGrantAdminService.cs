namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;

    public interface IGrantAdminService
    {
        /// <summary>
        /// Назначение администраторов гранта
        /// </summary>
        /// <param name="grantId">Идентификатор гранта</param>
        /// <param name="students">Список идентификаторов администраторов</param>
        /// <returns>Результат выполнения</returns>
        Task<DataResult> SetGrantAdmins(long grantId, long[] students);

        /// <summary>
        /// Получение администраторов гранта
        /// </summary>
        /// <param name="grantId">Идентификатор гранта</param>
        /// <returns>Список идентификаторов администраторов</returns>
        Task<IEnumerable<Student>> GetGrantAdmins(long grantId);

    }
}
