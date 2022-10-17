using System.Runtime.CompilerServices;
using Grant.Core.Enum;

namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.DbContext;
    using Core.Entities;
    using DataAccess;

    public class PersonalInfoService : BaseDomainService<PersonalInfo>, IPersonalInfoService
    {
        private GrantDbContext _db;
        private IStudentService studentService;
        private readonly IDomainService<PersonalInfoHistory> _historyDomain;

        public PersonalInfoService(IRepository<PersonalInfo> repository, IStudentService studentService, IDomainService<PersonalInfoHistory> historyDomain) : base(repository)
        {
            this.studentService = studentService;
            _historyDomain = historyDomain;
        }

        public Task<PersonalInfo> GetPersonalInfo(long userId)
        {
            return new Task<PersonalInfo>(null);
        }


        public async Task<PersonalInfo> GetStudentPersonalInfo(long studentId, long versionId)
        {
            var student = await this.studentService.Get(studentId);

            var info = await GetAll()
                    .Include(x => x.PassportPage1)
                    .Include(x => x.PassportPage2)
                    .Include(x => x.PassportPage3)
                    .Include(x => x.PassportPage4)
                    .Include(x => x.PassportPage5)
                    .Include(x => x.PassportPage6)
                    .Include(x => x.PassportPage7)
                    .Include(x => x.PassportPage8)
                    .Include(x => x.PassportPage9)
                    .Include(x => x.PassportPage10)
                    .Where(x => x.Id == student.PersonalInfoId).SingleOrDefaultAsync();

            if (versionId != 0)
            {
                var infoVersion = await _historyDomain.GetAll()
                    .Include(x => x.PassportPage1)
                    .Include(x => x.PassportPage2)
                    .Include(x => x.PassportPage3)
                    .Include(x => x.PassportPage4)
                    .Include(x => x.PassportPage5)
                    .Include(x => x.PassportPage6)
                    .Include(x => x.PassportPage7)
                    .Include(x => x.PassportPage8)
                    .Include(x => x.PassportPage9)
                    .Include(x => x.PassportPage10)
                    .Where(x => x.Id == versionId).FirstOrDefaultAsync();


                if (infoVersion != null)
                {
                    info = infoVersion.LoadHistory(info);
                }
            }

            return info;
        }

        public async Task<DataResult> IsPassportFilled(long studentId)
        {
            var data = await studentService.GetAll()
                    .Where(x => x.Id == studentId)
                    .Select(x => x.PersonalInfo)
                    .FirstOrDefaultAsync();

            if(data.Citizenship == Citizenship.Rf)
            {

                if ((data.Sex != Sex.Male && data.Sex != Sex.Female) ||
                    data.Birthday == null ||
                    string.IsNullOrEmpty(data.Birthplace) ||
                    string.IsNullOrEmpty(data.PassportSeries) ||
                    string.IsNullOrEmpty(data.PassportNumber) ||
                    string.IsNullOrEmpty(data.PassportIssuedBy) ||
                    string.IsNullOrEmpty(data.PassportIssuedByCode) ||
                    data.PassportIssueDate == null ||
                    string.IsNullOrEmpty(data.RegistrationRepublic) ||
                    (string.IsNullOrEmpty(data.RegistrationCity) && string.IsNullOrEmpty(data.RegistrationPlace)) ||
                    string.IsNullOrEmpty(data.RegistrationIndex) ||
                    string.IsNullOrEmpty(data.RegistrationStreet) ||
                    string.IsNullOrEmpty(data.RegistrationHouse) ||
                   // string.IsNullOrEmpty(data.RegistrationFlat) ||
                    string.IsNullOrEmpty(data.LiveRepublic) ||
                    (string.IsNullOrEmpty(data.LiveCity) && string.IsNullOrEmpty(data.LivePlace)) ||
                    string.IsNullOrEmpty(data.LiveIndex) ||
                    string.IsNullOrEmpty(data.LiveStreet) ||
                    string.IsNullOrEmpty(data.LiveHouse) ||
                   // string.IsNullOrEmpty(data.LiveFlat) ||
                    // string.IsNullOrEmpty(data.Phone) ||
                    (string.IsNullOrEmpty(data.Inn)) ||
                    !data.PassportPage9Id.HasValue)

                {
                    return DataResult.Ok(false);
                }
            }
            else
            {
                if ( !data.PassportPage1Id.HasValue ||
                     !data.PassportPage4Id.HasValue || 
                     !data.PassportPage5Id.HasValue ||
                     !data.PassportPage7Id.HasValue ||
                     !data.PassportPage10Id.HasValue)
                {
                    return DataResult.Ok(false);
                }
            }

            return DataResult.Ok(true);
        }

        /// <summary>
        /// Получим из строки хешей объекты
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public List<Core.DataFileResult> GetPassportScanLinks(string hash)
        {
            if (string.IsNullOrEmpty(hash))
            {
                return null;
            }

            var result = new List<Core.DataFileResult>();
            //так как пока все хеши файлов храним в одном поле через разделитель
            var hashes = hash.Split('|');

            using (this._db = new GrantDbContext())
            {
                var data = this._db.FilesInfo.Where(x => hashes.Contains(x.Guid)).ToList();
                result.AddRange(
                    data
                    .Select(x => 
                        new DataFileResult(x.VirtualPath, x.FileName, x.Guid, x.EditDate))
                        .ToList());
            }

            return  result;
        }

        //TODO: Это не нормально...
        /// <summary>
        /// Удаляем файл из базы
        /// </summary>
        /// <param name="info"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(PersonalInfo info, string hash)
        {
            var passportScanStringUpdateResult = await this.UpdatePassportScanString(info, hash);
            if (!passportScanStringUpdateResult.Success)
            {
                return false;
            }

            using (this._db = new GrantDbContext())
            {
                var fileInfoToDelete = await this._db.FilesInfo.Where(x => x.Guid.Equals(hash)).FirstOrDefaultAsync();
                this._db.FilesInfo.Remove(fileInfoToDelete);
                return await this._db.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// Удаляем запись о файле в сущности, путем вырезания из строки хеша
        /// </summary>
        /// <param name="info"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private Task<DataResult> UpdatePassportScanString(PersonalInfo info, string hash)
        {
            if (info.PassportScan.EndsWith(hash))
            {
                info.PassportScan = info.PassportScan.Replace(hash, string.Empty);
            }
            else
            {
                info.PassportScan = info.PassportScan.Replace(hash +"|", string.Empty);
            }
            return this.Update(info);
        }
    }
}
