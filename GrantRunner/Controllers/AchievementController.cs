using System;

namespace Grant.WebApi.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Core.DbContext;
    using Core.Entities;
    using DataAccess;
    using Grant.Utils.Extensions;
    using Request.Achievement;
    using Grant.Services.DomainService;
    using Grant.Services;

    //[RoutePrefix("api/Achievement")]

    [Authorize]
    public class AchievementController : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly IRepository<Achievement> _achievementRepo;
        private readonly ISession _session;
        private readonly IAchievementService service;
        private readonly IGrantEventService _grantEventService;

        public AchievementController()
        {
            _studentService = this.Container.GetInstance<IStudentService>();
            _achievementRepo = this.Container.GetInstance<IRepository<Achievement>>();
            _session = this.Container.Get<ISession>();
            service = this.Container.Get<IAchievementService>();
            _grantEventService = this.Container.GetInstance<IGrantEventService>();
        }

        [HttpGet]
        [Route("api/achievement/{id}")]
        public async Task<IHttpActionResult> Get(long id)
        {

            var achievements = await service.GetAllAsync();

            achievements.Foreach(x => x.FilesList = Utils.GetFilesListFromRow(x.Files, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);

            achievements.Foreach(x => x.ProofList = Utils.GetFilesListFromRow(x.ProofFile, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);
            return this.Ok(achievements);
        }

        [HttpGet]
        [Route("api/achievement/getbyid/{id}")]
        public async Task<IHttpActionResult> GetById(long id)
        {

            var achievement = await service.GetAsync(id);

            achievement.FilesList = Utils.GetFilesListFromRow(achievement.Files, ',');

            achievement.ProofList = Utils.GetFilesListFromRow(achievement.ProofFile, ',');

            return this.Ok(achievement);
        }

        [HttpGet]
        [Route("api/achievement/valid/{id}")]
        public async Task<IHttpActionResult> SetPassportValidState(long id)
        {

            var result = await this.service.SetValidState(id);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return this.Ok();
        }


        [HttpGet]
        [Route("api/achievement/getbystudent/{id}")]
        public async Task<IHttpActionResult> GetByStudent(long id)
        {
            if (id == 0)
            {
                id = (await _studentService.GetCurrent()).Id;
            }


            var year = DateTime.Now.Year;
            var previousYear = DateTime.Now.Year - 1;

            var achievements = (await service.GetAllAsync(id)).ToList(); //.Where(x => x.Year == year || x.Year == previousYear).ToList(); 

            achievements.Foreach(x => x.FilesList = Utils.GetFilesListFromRow(x.Files, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);
            achievements.Foreach(x => x.ProofList = Utils.GetFilesListFromRow(x.ProofFile, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);


            return this.Ok(achievements);
        }

        [HttpGet]
        [Route("api/oldachievement/getbystudent/{id}")]
        public async Task<IHttpActionResult> GetOldByStudent(long id)
        {
            if (id == 0)
            {
                id = (await _studentService.GetCurrent()).Id;
            }


            var year = DateTime.Now.Year;
            var previousYear = DateTime.Now.Year - 1;

            var achievements = (await service.GetAllAsync(id)).ToList(); //.Where(x => x.Year < previousYear).ToList();

            achievements.Foreach(x => x.FilesList = Utils.GetFilesListFromRow(x.Files, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);
            achievements.Foreach(x => x.ProofList = Utils.GetFilesListFromRow(x.ProofFile, ',')/*this._ahievementService.GetDataFilesList(x.Files)*/);

           // achievements.Foreach(x => x.IsNew = x.Year == 2019 || x.Year == 2018);

            return this.Ok(achievements);
        }

        //[HttpGet]
        //[Route("api/achievement/{studentId}/{id}")]
        //public async Task<IHttpActionResult> Get(long studentId, long id)
        //{
        //    studentId = (await _studentService.GetCurrent()).Id;

        //    var achievement = await _achievementRepo.GetAll()
        //        .Where(x => x.Student.Id == studentId)
        //        .Where(x => x.Id == id)
        //        .FirstOrDefaultAsync();
        //    achievement.FilesList = Utils.GetFilesListFromRow(achievement.Files, '|');
        //    // this._ahievementService.GetDataFilesList(achievement.Files);
        //    return this.Ok(achievement);
        //}

        [HttpPut]
        [Route("api/achievement/{id}")]
        public async Task<IHttpActionResult> PutAchievement(long id, Achievement achievement)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var oldAchievement = await service
                .GetAll()
                .Include(x=>x.Student)
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (oldAchievement == null)
            {
                return this.BadRequest("Не найдено достиженияы с таким Id");
            }

            var student = await _studentService.GetCurrent();
            var studentId = student.Id;

            

            if (achievement.StudentId != studentId)
            {

                await _grantEventService.CreateEvent(new Grant{ Id = 28 },
                    "Изменение достижения куратором",
                    $"Куратор {student.Name} {student.LastName} {student.Patronymic} отредактировал достижение студента {oldAchievement.Student.Name} {oldAchievement.Student.LastName} {oldAchievement.Student.Patronymic}: ",
                    Core.Enum.EventType.CuratorEditedAchievement, oldAchievement, achievement);

            }


            service.UpdateScore(achievement);
            achievement.ValidationState = Core.Enum.ValidationState.Unknown;

            try
            {
                await service.Update(achievement);
            }
            catch(Exception ex)
            {
                var t = ex.Message;
            }
          

            await service.UpdateScore(achievement.StudentId);

           

            return Ok(achievement);
        }

        [HttpPost]
        [Route("api/achievement/{studentId}")]
        [ResponseType(typeof(Achievement))]
        public async Task<IHttpActionResult> Post(long studentId, [FromBody]AchievementData data)
        {
            if (data == null)
            {
                return this.BadRequest();
            }
            var student = await _studentService.GetCurrent();
            studentId = student.Id;

            var context = this.Container.Get<ISession>().CurrentContext();
            Achievement entity;
            if (data.Id > 0)
            {
                entity = await _achievementRepo.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == data.Id);
                await _achievementRepo.Update(entity, true);
            }
            else
            {
                entity = new Achievement
                {
                    StudentId = studentId
                };
                context.Achievements.Add(entity);
            }

            Utils.MergeChanges(entity, data);
            service.UpdateScore(entity);
            entity.ValidationState = Core.Enum.ValidationState.Unknown;
            this.service.SaveOrUpdate(entity);
            //await context.SaveChangesAsync();

            await service.UpdateScore(entity.StudentId);

            return this.Ok(entity);
        }


        [HttpDelete]
        [Route("api/achievement/{id}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            var stud = await _studentService.GetCurrent();
            if (stud != null)
            {

                var dataResult = await service.Delete(id);

                await service.UpdateScore(stud.Id);

                return this.Ok();
            }
            else
            {
                return BadRequest("Не найден студент");
            }
        }

        [HttpGet]
        [Route("api/achievement/isachievementFilled/{studentId}")]
        public async Task<Boolean> IsAchievementFilled(long studentId)
        {
            if (studentId == 0)
            {
                studentId = (await _studentService.GetCurrent()).Id;
            }

            var result = await service.IsAchievementFilled(studentId);

            if (result.Success)
            {
                return (bool)result.Data;
            }
            else
            {
                throw new Exception(result.Message);
            }
        }
    }
}