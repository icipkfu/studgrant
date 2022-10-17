namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;
    using Models;

    public interface IPersonalInfoService
    {
        Task<PersonalInfo> GetPersonalInfo(long userId);

        List<Core.DataFileResult> GetPassportScanLinks(string hash);

        Task<bool> DeleteFile(PersonalInfo info, string hash);

        Task<PersonalInfo> GetStudentPersonalInfo(long studentId, long versionId);

        Task<DataResult> IsPassportFilled(long studentId);

    }
}
