using System;
using System.Data.Entity;
using Grant.Core;
using Grant.Core.DbContext;
using Grant.Core.Entities;
using Grant.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grant.Core.Enum;

namespace Grant.Services.DomainService
{
    public class AchievementService : BaseDomainService<Achievement>, IAchievementService
    {
        private IFileManager fileManager;
        private IStudentService studentService;
        private GrantDbContext _db;
        private IAchievementScore scoreService;
        private IRepository<ValidationHistory> validationHistRepo;
     
        HashSet<string> imageExt = new HashSet<string> { ".jpg", ".png" };

        public AchievementService(IRepository<Achievement> repository, IFileManager fileManager, IStudentService studentService, IAchievementScore scoreService, IRepository<ValidationHistory> validationHistRepo) : base(repository)
        {
            this.fileManager = fileManager;
            this.studentService = studentService;
            this.scoreService = scoreService;
            this.validationHistRepo = validationHistRepo;
        }

        public async Task<DataResult> SetValidState(long id)
        {
            var item = await Get(id);

            if (item == null)
            {
                return DataResult.Failure("Достижение с таким id не найдено");
            }

            var moderator = await studentService.GetCurrent();

            if (item.ValidationState != ValidationState.Valid)
            {
                var rec = new ValidationHistory
                {
                    ValidationUserId = item.StudentId,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Valid,
                    Target = ValidationTarget.Achievement
                };

                await validationHistRepo.Create(rec);
            }

            item.ValidationState = ValidationState.Valid;
            item.ValidationComment = null;

            await Update(item);

            return DataResult.Ok();
        }

        public async override Task<IQueryable<Achievement>> GetAllAsync()
        {

            var curStud = await studentService.GetCurrent();

            var result = await GetAll().Where(x => x.StudentId == curStud.Id).OrderByDescending(x=>x.Year).ToListAsync();
            
            foreach (var achievement in result)
            {
                if (!String.IsNullOrEmpty(achievement.Files))
                {
                    var fileArr = achievement.Files.Split(',');

                    foreach (var file in fileArr)
                    {
                        var image = await fileManager.Get(file);

                        if (image != null && imageExt.Contains(image.Extension))
                        {
                            achievement.ImageLink = image.VirtualPath;
                        }
                        break;
                    }
                }

                if (string.IsNullOrEmpty(achievement.ImageLink))
                {
                    achievement.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
                }
            }

            return result.AsQueryable();
        }

        public async Task<Achievement> GetAsync(long id)
        {

            var curStud = await studentService.GetCurrent();

            var achievement = await GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();

            if(achievement == null)
            {
                return achievement;
            }

         
             if (!String.IsNullOrEmpty(achievement.Files))
             {
                    var fileArr = achievement.Files.Split(',');

                    foreach (var file in fileArr)
                    {
                        var image = await fileManager.Get(file);

                        if (image != null && imageExt.Contains(image.Extension))
                        {
                            achievement.ImageLink = image.VirtualPath;
                        }
                        break;
                    }
             }

             if (string.IsNullOrEmpty(achievement.ImageLink))
             {
                    achievement.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
             }
            
       
            return achievement;
        }

        public async Task<IQueryable<Achievement>> GetAllAsync(long studentId)
        {

            var result = await GetAll().Where(x => x.StudentId == studentId).OrderByDescending(x => x.Year).ToListAsync();

            foreach (var achievement in result)
            {
                if (!String.IsNullOrEmpty(achievement.Files))
                {
                    var fileArr = achievement.Files.Split(',');

                    foreach (var file in fileArr)
                    {
                        var image = await fileManager.Get(file);

                        if (image != null && imageExt.Contains(image.Extension))
                        {
                            achievement.ImageLink = image.VirtualPath;
                        }
                        break;
                    }
                }

                if (string.IsNullOrEmpty(achievement.ImageLink))
                {
                    achievement.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
                }
            }

            return result.AsQueryable();
        }

        ///// <summary>
        ///// Удаление вместе с инфромацией о файлах
        ///// </summary>
        ///// <param name="id"></param>
        ///// <remarks>Но из каталога файлы не удаляются</remarks>
        ///// <returns>Результат выполнения операции</returns>
        //public override async Task<DataResult> Delete(long id)
        //{
        //    using (this._db = new GrantDbContext())
        //    {
        //        var achieve = await this._db.Achievements.FindAsync(id);
        //        if(achieve != null && string.IsNullOrEmpty(achieve.Files))
        //        {
        //            var filesHashList = achieve.Files.Split('|');
        //            var files = this._db.FilesInfo.Where(x => achieve.Files.Contains(x.Guid)).ToList();
        //            foreach(var f in files)
        //            {
        //                this._db.Entry(f).State = System.Data.Entity.EntityState.Deleted;
        //            }
        //            await this._db.SaveChangesAsync();
        //        }
        //    }
                
        //    return await base.Delete(id);
        //}

        public List<DataFileResult> GetDataFilesList(string hash)
        {
           if (string.IsNullOrEmpty(hash))
            {
                return null;
            }

            var result = new List<DataFileResult>();
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

        public void UpdateScore(Achievement ach)
        {

            var list = new List<Achievement>
            {
                ach
            };

            ach.Score = scoreService.GetScore(list);
        }

        public async Task UpdateScore(long studentId)
        {
            var achievements = await GetAll().Include(x => x.Student).Where(x => x.Student.Id == studentId).Where(x=>x.Year >= 2018).ToListAsync();


            if (achievements.Any())
            {
                var score = scoreService.GetScore(achievements);

                var stud = achievements.First().Student;

                if (stud.Score != score)
                {
                    stud.Score = score;
                    await studentService.Update(stud);
                }
            }
            else
            {
                var score = 0;

                var stud = await studentService.Get(studentId);

                if (stud.Score != score)
                {
                    stud.Score = score;
                    await studentService.Update(stud);
                }
            }
        }

        public async Task<DataResult> IsAchievementFilled(long studentId)
        {
            var data = await GetAll().Where(x => x.StudentId == studentId).ToListAsync();

            //DateTime date = new DateTime(2016, 5, 1);

           // var freshAchievements = data.Where(x => x.CreateDate > date).ToList();

            if (!data.Any())
            {
                return DataResult.Ok(false);
            }

            return DataResult.Ok(true);
        }

    }
}
