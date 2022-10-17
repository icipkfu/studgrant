using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Grant.Core;
using Grant.Core.Enum;
using Grant.Core.UserIdentity;
using Grant.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Grant.WebApi.Controllers
{
    using System.Net;
    using System.Threading.Tasks;

    using System.Web.Http.Description;
    using Grant.Core.Entities;
    using Grant.Services.DomainService;
    using Grant.Utils.Extensions;
    using System.Collections.Generic;
    using Request;
    using Services;

     [Authorize]
    public class StudentsController : BaseController
    {

         private ApplicationUserManager _userManager;

        public StudentsController()
        {
            _domain = this.Container.Get<IDomainService<PersonalInfo>>();
            _service = this.Container.Get<IStudentService>();
            studentRepo = this.Container.Get<IRepository<Student>>();
            grantService = this.Container.Get<IGrantStudentService>();
            bankService = this.Container.Get<IBankFilialService>();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IStudentService _service;
         private IGrantStudentService grantService;
        private IBankFilialService bankService;
        private readonly IDomainService<PersonalInfo> _domain;
        private readonly IRepository<Student> studentRepo; 

        private IStudentService Service
        {
            get { return (_service ?? (_service = Container.Get<IStudentService>())); }
        }

        // GET: api/Students
         [HttpGet]
        public async Task<IEnumerable<Student>> GetStudents()
        {
            return await this.Service.GetAllAsync();
        }

        [HttpGet]
        [ResponseType(typeof (Student))]
        public async Task<IHttpActionResult> GetStudent(long id)
        {
            var student = await this.Service.GetCurrent();

            if (student == null)
            {
                return BadRequest("Нет студента с данным id");
            }

            return this.Ok(student);
        }

        // PUT: api/Students/5
        [HttpPut]
        [ResponseType(typeof (void))]
        public async Task<IHttpActionResult> PutStudent([FromBody]StudentData data)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (data == null)
            {
                return this.BadRequest();
            }

            var student = await studentRepo.GetAll().Where(x=>x.Id == data.Id).SingleOrDefaultAsync() ?? new Student();


            var fioChange = String.Compare(student.Name, data.Name, StringComparison.CurrentCultureIgnoreCase) != 0 ||
                String.Compare(student.LastName, data.LastName, StringComparison.CurrentCultureIgnoreCase) != 0 ||
                String.Compare(student.Patronymic, data.Patronymic, StringComparison.CurrentCultureIgnoreCase) != 0;

            Utils.MergeChanges(student, data);

            if (student.PassportState == ValidationState.Valid && fioChange)
            {
                student.PassportState = ValidationState.Unknown;
            }

            if (data.UniversityId > 0 )
            {
                student.UniversityId = data.UniversityId;
            }

            student.ProfileEditDate = DateTime.Now;
          
            if (student.Id > 0)
            {
                await Service.Update(student);
            }
            else
            {
                await Service.Create(student);
            }

            student = await Service.Get(student.Id);

            return this.Ok(student);
        }

        // POST: api/Students
         [HttpPost]
        [ResponseType(typeof (Student))]
        public async Task<IHttpActionResult> PostStudent(Student student)
        {
            student.UserIdentityId = HttpContext.Current.User.Identity.GetUserId();


            var info = new PersonalInfo();
            await _domain.Create(info);
            student.PersonalInfoId = info.Id;

            var result = await this.Service.Create(student);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtRoute("DefaultApi", new {id = student.Id}, student);
        }

        [HttpGet]
        [Route("api/students/getbyname/{name}")]
        public async Task<IEnumerable<Student>> GetByName(string name, [FromUri] bool exceptAdmins = false)
        {
            return await Service.GetByName(name, exceptAdmins);
        }

        [HttpGet]
        [Route("api/students/bankfilialname/{name}")]
        public async Task<IEnumerable<BankFilial>> GetByFilialName(string name, [FromUri] bool exceptAdmins = false)
        {
            if (name != "-1")
            {

                return bankService.GetAll().Where(x => x.FilialName.ToUpper().Contains(name.ToUpper())).OrderBy(x => x.FilialName);
            }

            return bankService.GetAll().OrderBy(x => x.FilialName);
        }


        [HttpPut]
        [Route("api/students/getbyname")]
        public async Task<IEnumerable<Student>> GetByName([FromBody] StudentFilter search)
        {
            return await Service.GetFiltered(search);
        }

        [HttpGet]
        [Route("api/students/getbyuniversity/{id}")]
        public async Task<IQueryable<Student>> GetByUniversity(long id)
        {
            return  await Service.GetStudents(id);
        }

        [HttpGet]
        [ResponseType(typeof(Student))]
        [Route("api/students/getinfo/{id}")]
        public async Task<IHttpActionResult> GetInfo(long id)
        {
            Student student = null;

            if(id != 3656)
            {
                if (id != 0)
                {
                    student = await this.Service.Get(id);
                }
                else
                {
                    student = await this.Service.GetCurrent();
                }
            }
            else
            {
                throw new Exception();
            }
          
           
            if (student == null)
            {
                return BadRequest("Нет студента с данным id");
            }

            if (string.IsNullOrEmpty(student.Email))
            {
                student.Email = await UserManager.GetEmailAsync(student.UserIdentityId);
            }

            return this.Ok(student);
        }


        [HttpGet]
        [Route("api/students/setpaspvalid/{id}")]
        public async Task<IHttpActionResult> SetPassportValidState(long id)
        {

            var result = await this.Service.SetPassportValidState(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpGet]
        [Route("api/students/setincomevalid/{id}")]
        public async Task<IHttpActionResult> SetIncomeValidState(long id)
        {

            var result = await this.Service.SetIncomeValidState(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("api/students/setpaspinvalid/{id}/{force}")]
        public async Task<IHttpActionResult> SetPassportInvalidState(long id, bool force, [FromBody] ValidationMassage msg)
        {
            var result = await this.grantService.SetPassportInvalidState(id, msg.Message, force);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpPut]
        [Route("api/students/setincomeinvalid/{id}/{force}")]
        public async Task<IHttpActionResult> SetIncomeInvalidState(long id, bool force, [FromBody] ValidationMassage msg)
        {
            var result = await this.grantService.SetIncomeInvalidState(id, msg.Message, force);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("api/students/setachinvalid/{id}/{force}")]
        public async Task<IHttpActionResult> SetAchievementInvalidState(long id, bool force, [FromBody] ValidationMassage msg)
        {
            var result = await this.grantService.SetAchievementInvalidState(id, msg.Message, force);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpGet]
        [Route("api/students/setbookvalid/{id}")]
        public async Task<IHttpActionResult> SetBooktValidState(long id)
        {
            var result = await this.Service.SetBookValidState(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("api/students/setbookinvalid/{id}/{force}")]
        public async Task<IHttpActionResult> SetBookValidState(long id, bool force, [FromBody] ValidationMassage msg)
        {
            var result = await this.grantService.SetBookInvalidState(id, msg.Message, force);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpGet]
        [Route("api/students/getroleinfo")]
        public async Task<RoleInfo> GetRoleInfo()
        {
            return await this.Service.GetCurrentRole();
        }

        /*[HttpGet]
        [Route("api/students/{studentId}/personalinfo")]
        public async Task<IHttpActionResult> GetPersonalInfo(long studentId, PersonalInfo personalInfo)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var student = this.service.GetAll()
                .Where(x => x.Id == studentId)
                .Select(x => (long?)x.Id)
                .FirstOrDefault();

            return this.Json(student);
        }

        [HttpPost]
        [Route("api/students/{studentId}/personalinfo")]
        public async Task<IHttpActionResult> PostPersonalInfo(long studentId, PersonalInfo personalInfo)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            //var result = await service.

            return this.BadRequest();
        }*/

        // DELETE: api/Students/5
        [ResponseType(typeof (Student))]
        public async Task<IHttpActionResult> DeleteStudent(long id)
        {
            var result = await this.Service.Delete(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok((Student) result.Data);
        }

        [HttpGet]
        [Route("api/students/isprofileFilled/{id}")]
        public async Task<Boolean> IsProfileFilled(long id)
        {
            if (id == 0)
            {
                id = (await this.Service.GetCurrent()).Id;
            }

            var result = await this.Service.IsProfileFilled(id);

            if (result.Success)
            {
                return (bool)result.Data;
            }
            else
            {
                throw new Exception(result.Message);
            }
        } 

        [HttpGet]
        [Route("api/students/isrecordbookFilled/{id}")]
        public async Task<Boolean> IsRecordBookFilled(long id)
        {
            if (id == 0)
            {
                id = (await this.Service.GetCurrent()).Id;
            }

            var result = await this.Service.IsRecordBookFilled(id);

            if (result.Success)
            {
                return (bool)result.Data;
            }
            else
            {
                throw new Exception(result.Message);
            }
        }

        [HttpGet]
        [Route("api/students/isincomeFilled/{id}")]
        public async Task<Boolean> IsIncomeFilled(long id)
        {
            if (id == 0)
            {
                id = (await this.Service.GetCurrent()).Id;
            }

            var result = await this.Service.IsIncomeFilled(id);

            if (result.Success)
            {
                return (bool)result.Data;
            }
            else
            {
                throw new Exception(result.Message);
            }
        }

        [HttpGet]
        [Route("api/students/getmoderators")]
        public async Task<IEnumerable<Student>> GetModerators()
        {
            return await Service.GetModerators();
        }

        [HttpPut]
        [Route("api/students/setmoderators")]
        public async Task<DataResult> SetModerators([FromBody] ModeratorIdList list )
        {
            return await Service.SetModerators(list.Ids);
        }


        [HttpPut]
        [Route("api/students/getvalidationstat")]
        public async Task<DataResult> GetValidationStat([FromBody] EventFilter filter)
        {
            return await Service.GetValidationStat(filter);
        }

       [HttpGet]
       [Route("api/students/getuserstat/{id}")]
        public async Task<DataResult> GetUserStat(long id)
        {
            return await Service.GetUserStat(id);
        }


        [HttpGet]
        [Route("api/students/getvalidatorname/{id}/{target}")]
        public async Task<DataResult> GetValidatorName(long id, int target)
        {
            var targ = ValidationTarget.PersonalInfo;
            if (target == 2)
            {
                targ = ValidationTarget.RecordBook;
            }

            return await Service.GetValidatorName(targ, id);
        }

        [HttpGet]
        [Route("api/students/getvalidationhistory/{id}/{target}")]
        public async Task<DataResult> GetValidationHistory(long id, int target)
        {
            var targ = ValidationTarget.PersonalInfo;
            if (target == 2)
            {
                targ = ValidationTarget.RecordBook;
            }

            return await Service.GetValidationHistory(targ, id);
        }

        [HttpGet]
        [Route("api/students/getcontactinfo")]
        public async Task<DataResult> GetContactInfo()
        {
            return await Service.GetContactInfo();
        }




         
    }
}