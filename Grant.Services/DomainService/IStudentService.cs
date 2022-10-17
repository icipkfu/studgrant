using System.Collections.Generic;
using System.Linq;
using Grant.Core.Enum;

namespace Grant.Services.DomainService
{
    using System.Threading.Tasks;
    using Core.Entities;
    using Core;

    public interface IStudentService : IDomainService<Student>
    {
        Task<Student> GetCurrent();

        Task<IQueryable<Student>> GetStudents(long id);

        Task<DataResult> SetPassportValidState(long id);

       // Task<DataResult> SetPassportInvalidState(long id, string message);

        Task<DataResult> SetBookValidState(long id);

       // Task<DataResult> SetBookInvalidState(long id, string message);

        Task<RoleInfo> GetCurrentRole();

        Task<IEnumerable<Student>> GetByName(string name, bool exceptAdmins =false);

        Task<IEnumerable<Student>> GetFiltered(StudentFilter search);

        Task<DataResult> IsProfileFilled(long id);

        Task<IEnumerable<Student>> GetModerators();

        Task<DataResult> SetModerators(List<long> students);
        Task<DataResult> IsRecordBookFilled(long id);
        Task<DataResult> IsIncomeFilled(long id);

        Task<DataResult> GetValidationStat(EventFilter filter);

        Task<DataResult> GetUserStat(long grantId);

        Task<DataResult> GetValidatorName(ValidationTarget target, long studentId);

        Task<DataResult> GetValidationHistory(ValidationTarget target, long studentId);

        Task<DataResult> GetContactInfo();

        Task<DataResult> SetIncome(long id, int income);

        Task<DataResult> SetIncomeValidState(long id);

    }
}
