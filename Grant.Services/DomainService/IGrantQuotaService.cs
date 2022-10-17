using System.Collections.Generic;

namespace Grant.Services.DomainService
{
    using System.Linq;
    using System.Threading.Tasks;
    using Grant.Core.Entities;
    using Grant.Core; 

    public interface IGrantQuotaService : IDomainService<GrantQuota>
    {
        Task<List<GrantQuota>> GetGrantQuotas(long grantId);

        Task Update(long grantId, GrantQuota[] quotas);

        Task<GrantQuota> GetUniversityQuota(long grantId, long univerId);

        Task<GrantQuota> GetUniversityFullQuota(long grantId, long univerId);

        Task<DataResult> AddWinnerReport(long grantId, long univerId, string report);

        Task<DataResult> DeleteWinnerReport(long grantId, long univerId);

        Task<DataResult> AddAdditionalWinnerReport(long grantId,  long univerId,  string report);

        Task<DataResult> DeleteAdditionalWinnerReport(long grantId, long univerId);
    }
}
