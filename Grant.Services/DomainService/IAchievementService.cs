namespace Grant.Services.DomainService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Grant.Core;
    using System.Collections.Generic;
    using Grant.Core.Entities;

    public interface IAchievementService : IDomainService<Achievement>
    {
        List<DataFileResult> GetDataFilesList(string hash);

        Task<IQueryable<Achievement>> GetAllAsync(long studentId);

        Task<DataResult> IsAchievementFilled(long studentId);

        Task UpdateScore(long studentId);

        Task<Achievement> GetAsync(long id);

        Task<DataResult> SetValidState(long id);

        void UpdateScore(Achievement ach);
    }
}
