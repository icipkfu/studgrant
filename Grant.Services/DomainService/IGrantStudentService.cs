using System;

namespace Grant.Services.DomainService
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Grant.Core.Entities;
    using Grant.Core;

    public interface IGrantStudentService : IDomainService<GrantStudent>
    {

        Task<DataResult> GetAdditionalStudents(AdditionalWinnerFilter filter);

        Task<DataResult> ChooseWinner(long grantId, long studentId);

        Task<DataResult> CancelWinner(long grantId, long studentId);

        Task<DataResult> ChooseAdditionalWinner(long grantId, long studentId);

        Task<DataResult> GetGrantStudents(long grantId, long universityId, GrantStudentFilter filter);

        Task<DataResult> CancelAdditionalWinner(long grantId, long studentId);

        Task<DataResult> DeclineGrant(long grantId);

        Task<DataResult> AcceptGrant(long grantId, bool isSocialActive, bool isSocialHelp);

        Task<DataResult> GetStudentsReport(long grantid, long univerId, bool additional = false);

        Task<DataResult> GetWinnersReport(long grantid, long univerId, bool additional = false);

        Task<DataResult> GetFullGrantReport(long grantid,  bool? onlyNewWinners, bool additional = false, long univerId = 0);

        Task<DataResult> GetMainFullGrantReport(long grantid,  bool? onlyNewWinners, bool additional = false, long univerId = 0);
       
        Task<DataResult> GetFullGrantOtherReport(long grantid, bool? onlyNewWinners, long univerId = 0, long activ =0);

        Task<Boolean> IsParticipant(long grantId);

        Task<IEnumerable<University>> GetGrantUnivers(long grantId, bool additional);

        Task<DataResult> GetStudentsDbfReport(long grantid,  bool? onlyNewWinners, long univerId, bool additional = false);

        Task<DataResult> GetStat(long grantId);

        Task<DataResult> GetWinnersList(long grantid, bool? onlyNewWinners);

        Task<DataResult> GetAllAdditionalReport(long grantid, bool? onlyNewWinners);

        Task<DataResult> GetUserGrants(long studentId);

        Task<DataResult> SetPassportInvalidState(long id, string message, bool force = false);

        Task<DataResult> SetBookInvalidState(long id, string message, bool force = false);

        Task<DataResult> SetIncomeInvalidState(long id, string message, bool force = false);

        Task<DataResult> SetAchievementInvalidState(long id, string message, bool force = false);
    }
}
