using System;
using System.Diagnostics;
using Grant.Core.Enum;
using Grant.Utils.Extensions;

namespace Grant.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc.Async;
    using Core.Entities;
    using Services.DomainService;
    using Services;
    using Request;

   // [RoutePrefix("api/personalInfo/{studentId}")]

     [Authorize]
    public class PersonalInfoController : BaseController
    {
        private readonly IStudentService _studentDomain;
        private readonly IDomainService<PersonalInfo> _domain;
        private readonly IDomainService<PersonalInfoHistory> _historyDomain;
        private readonly IPersonalInfoService _personalInfoService;

        public PersonalInfoController()
        {
            this._domain = this.Container.GetInstance<IDomainService<PersonalInfo>>();
            this._studentDomain = this.Container.GetInstance<IStudentService>();
            this._personalInfoService = this.Container.GetInstance<IPersonalInfoService>();
            this._historyDomain = this.Container.GetInstance<IDomainService<PersonalInfoHistory>>();
        }

        [HttpGet]
        [Route("api/personalInfo/{studentId}")]
        public async Task<IHttpActionResult> Get(long studentId)
        {
            var info = await this.GetCurrentInfo();
            info.PassportScanLinks = this._personalInfoService.GetPassportScanLinks(info.PassportScan);
            info.PassportScans = info.PassportScans;

            return this.Ok(info);
        }

        [HttpGet]
        [Route("api/personalInfo/getbystudent/{studentId}/{versionId}")]
        public async Task<IHttpActionResult> GetStudentInfo(long studentId, long versionId)
        {

            if (studentId == 0)
            {
                studentId = (await _studentDomain.GetCurrent()).Id;
            }

            var info = await _personalInfoService.GetStudentPersonalInfo(studentId, versionId);
            info.PassportScanLinks = this._personalInfoService.GetPassportScanLinks(info.PassportScan);

            return this.Ok(info);
        }

        [HttpGet]
        [Route("api/personalInfo/getVersions/{studentId}")]
        public async Task<IHttpActionResult> GetVersions(long studentId)
        {
            var versions = (await _historyDomain.GetAll().Where(x => x.StudentId == studentId).Select(x => new
            {
                x.Id,
                x.EditTime
            }).OrderByDescending(x => x.EditTime).ToListAsync())
                .Select(x => new
                {
                    x.Id,
                    EditTime = x.EditTime.ToString("dd.MM.yyyy HH:mm:ss")
                }).ToList();


            return this.Ok(versions);
        }

        [HttpGet]
        [Route("api/personalInfo/ispassportFilled/{studentId}")]
        public async Task<Boolean> IsPassportFilled(long studentId)
        {
            if (studentId == 0)
            {
                studentId = (await _studentDomain.GetCurrent()).Id;
            }

            var result = await _personalInfoService.IsPassportFilled(studentId);

            if (result.Success)
            {
                return (bool) result.Data;
            }
            else
            {
                throw new Exception(result.Message);
            }
        }



       // Task<DataResult> IsPassportFilled(long studentId)

        [HttpPost]
        [Route("api/personalInfo/{studentId}")]
        public async Task<IHttpActionResult> Post(long studentId, [FromBody]PersonalInfoData data)
        {

            var now = DateTime.Now;
       
            var student = (await this._studentDomain.GetCurrent());
            if (data == null)
            {
                return this.BadRequest();
            }

            if (student == null)
            {
                return this.NotFound();
            }

            PersonalInfo info = null;

            if (student.PersonalInfoId != 0)
            {
                info = await this._domain.Get(student.PersonalInfoId);
            }
            else
            {
                info = new PersonalInfo();
            }
            
            Utils.MergeChanges(info, data);

            this._domain.SaveOrUpdate(info);

            var persInfoHist = new PersonalInfoHistory();
            persInfoHist.MakeHistory(info, student.Id, now);
            _historyDomain.SaveOrUpdate(persInfoHist);

            var isFilled = await _personalInfoService.IsPassportFilled(student.Id);

            student.PassportState = isFilled.Data.ToBool() ? ValidationState.Unknown : ValidationState.NotFilled;

            if (student.PersonalInfoId == 0)
            {
                student.PersonalInfoId = info.Id;
            }

            student.PersonalDataEditDate = now;

            await this._studentDomain.Update(student);

            info = await this.GetCurrentInfo();

            return this.Ok(info);
        }

        [HttpGet]
        [Route("api/personalInfo/add/{hash}")]
        public async Task<IHttpActionResult> Add(string hash)
        {
            var now = DateTime.Now;
            var student = await _studentDomain.GetCurrent();

            PersonalInfo info = null;

            if (student.PersonalInfoId != 0)
            {
                info = await this._domain.Get(student.PersonalInfoId);
            }
            else
            {
                info = new PersonalInfo();
            }

            var files = info.PassportScan;

            if (!string.IsNullOrEmpty(files) && files.Length > 0)
            {
                files = String.Format("{0}|{1}", files, hash);
            }
            else
            {
                files = hash;
            }

            info.PassportScan = files;

            _domain.SaveOrUpdate(info);

            var persInfoHist = new PersonalInfoHistory();
            persInfoHist.MakeHistory(info, student.Id, now);
            _historyDomain.SaveOrUpdate(persInfoHist);

            if (student.PersonalInfoId != info.Id)
            {
                student.PersonalInfoId = info.Id;
                await this._studentDomain.Update(student);
            }

            info.PassportScanLinks = this._personalInfoService.GetPassportScanLinks(info.PassportScan);

            return this.Ok(info.PassportScanLinks);
        }

        

        private async Task<PersonalInfo> GetCurrentInfo()
        {
            var student = await this._studentDomain.GetCurrent();

            return
                await _domain.GetAll()
                    .Include(x => x.PassportPage1)
                    .Include(x => x.PassportPage2)
                    .Include(x => x.PassportPage3)
                    .Include(x => x.PassportPage4)
                    .Include(x => x.PassportPage5)
                    .Include(x=>x.PassportPage6)
                    .Include(x => x.PassportPage7)
                    .Include(x => x.PassportPage8)
                     .Include(x => x.PassportPage9)
                      .Include(x => x.PassportPage10)
                    .Where(x => x.Id == student.PersonalInfoId).SingleOrDefaultAsync();
        }
    }
}