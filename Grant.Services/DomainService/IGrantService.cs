namespace Grant.Services.DomainService
{
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;
    using System.Collections.Generic;

    public interface IGrantService :  IDomainService<Grant>
    {
        Task<DataResult> ReturnToDraft(long id);

        Task<DataResult> StartGrant(long id);

        Task<DataResult> CloseRegistration(long id);

        Task<DataResult> OpenWinnersSelection(long id);

        Task<DataResult> CloseWinnersSelection(long id);

        Task<DataResult> OpenAdditionalWinnersSelection(long id);

        Task<DataResult> CloseAdditionalWinnersSelection(long id);

        Task<DataResult> OpenDelivery(long id);

        Task<DataResult> CancelDelivery(long id);

        Task<DataResult> OpenFinal(long id);

        Task<DataResult> CancelFinal(long id);

        Task<IEnumerable<Student>> GetAdmins(long id);

        Task<DataResult> GetGrantList();

        Task<DataResult> СhangeCanAddReport(long id, bool option);

        Task<object> GetGrantRegChart(long id);
    }
}
