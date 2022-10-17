using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Async;
using Grant.Core.Enum;
using Grant.Utils.Extensions;
using Ionic.Zip;

namespace Grant.Services.DomainService
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core;
    using Core.Context;
    using Core.Entities;
    using DataAccess;

    public class StudentService : BaseDomainService<Student>, IStudentService
    {
        private IFileManager fileManager;
        private IRepository<University> universityService;
        private IRepository<GrantAdmin> grantAdminRepo;
        private IRepository<GrantStudent> grantstudentRepo;
        private IRepository<ValidationHistory> validationHistRepo;

        public StudentService(IRepository<Student> repository,  IRepository<ValidationHistory> validationHistRepo, IFileManager fileManager, IRepository<GrantAdmin> grantAdminRepo, IRepository<University> universityService, IRepository<GrantStudent> grantstudentRepo) : base(repository)
        {
            this.fileManager = fileManager;
            this.grantAdminRepo = grantAdminRepo;
            this.universityService = universityService;
            this.grantstudentRepo = grantstudentRepo;
            this.validationHistRepo = validationHistRepo;
        }

        public async override Task<Student> Get(long id)
        {
            var entity = await base.GetAll().Include(x => x.University).Include(x=>x.PersonalInfo).Where(x => x.Id == id).SingleOrDefaultAsync();

             if (entity != null && !String.IsNullOrEmpty(entity.ImageFile))
                {
                    var image = await fileManager.Get(entity.ImageFile);

                    if (image != null)
                    {
                        entity.ImageLink = image.VirtualPath;
                    }
                }

             if (entity != null && String.IsNullOrEmpty(entity.ImageLink))
             {
                 entity.ImageLink = entity.PersonalInfo.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
             }

             entity.RecordBookEditDate = entity.RecordBookEditDate.AddHours(3);
             entity.PersonalDataEditDate = entity.PersonalDataEditDate.AddHours(3);



            return entity;
        }

        public async Task<IEnumerable<Student>> GetModerators()
        {
            var students = await GetAll().Where(x => x.Role == Role.ZoneModerator).ToListAsync();

            foreach (var student in students)
            {
                if (!String.IsNullOrEmpty(student.ImageFile))
                {
                    var image = await fileManager.Get(student.ImageFile);

                    if (image != null)
                    {
                        student.ImageLink = image.VirtualPath;
                    }
                }

                if (String.IsNullOrEmpty(student.ImageLink))
                {
                    student.ImageLink = student.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
                }
            }

            return students;
        }

        public async Task<DataResult> SetModerators(List<long> students)
        {

            var newModers = await GetAll().Where(x => students.Contains(x.Id) && x.Role != Role.ZoneModerator).ToListAsync();

            var oldModers = await GetAll().Where(x => x.Role == Role.ZoneModerator && !students.Contains(x.Id)).ToListAsync();

            foreach (var rec in oldModers)
            {
                rec.Role = Role.RegistredUser;
                await base.Update(rec, true);
            }

            foreach (var rec in newModers)
            {
                rec.Role = Role.ZoneModerator;
                await base.Update(rec, true);
            }

            await SaveChangesAsync();

            return DataResult.Ok();
        }


        public async override Task<DataResult> Create(Student student)
        {
            return await base.Create(student);
        }

        public async Task<Student> GetCurrent()
        {
            var curUSer = ApplicationContext.Current.CurUserId();

            var entity = await GetAll().SingleOrDefaultAsync(x => x.UserIdentityId == curUSer);

            if (entity == null)
            {
                return null;
            }

            if (!String.IsNullOrEmpty(entity.ImageFile))
            {
                var image = await fileManager.Get(entity.ImageFile);

                if (image != null)
                {
                    entity.ImageLink = image.VirtualPath;
                }
            }
            
            if (String.IsNullOrEmpty(entity.ImageLink))
                {
                    entity.ImageLink = entity.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
                }

            if (entity.UniversityId > 0)
            {
                entity.University =
                    await universityService.GetAll().Where(x => x.Id == entity.UniversityId).SingleOrDefaultAsync();
            }

            if(entity.RecordBookEditDate.Year > 2000)
            {
                entity.RecordBookEditDate = entity.RecordBookEditDate.AddHours(3);
            }

            if (entity.PersonalDataEditDate.Year > 2000)
            {
                entity.PersonalDataEditDate = entity.PersonalDataEditDate.AddHours(3);
            }

            if (entity.IncomeEditDate.Year > 2000)
            {
                entity.IncomeEditDate = entity.IncomeEditDate.AddHours(3);
            }


            return entity;
        }

        public async Task<RoleInfo> GetCurrentRole()
        {
            var curStud = await GetCurrent();

            var info = new RoleInfo
            {
                Id = curStud.Id,
                Role = curStud.Role,
                GrantsAdmin = await grantAdminRepo.GetAll()
                    .Where(x => x.StudentId == curStud.Id)
                    .Select(x => x.GrantId)
                    .Distinct()
                    .ToListAsync(),
                UniversCurator = await universityService.GetAll()
                    .Where(x => x.CuratorId == curStud.Id)
                    .Select(x => x.Id)
                    .Distinct()
                    .ToListAsync()
            };


            return info;
        }

        public async Task<IQueryable<Student>> GetStudents(long id)
        {
            var result = base.GetAll().Where(x => x.UniversityId == id).ToList();

            foreach (var student in result)
            {
                if (!String.IsNullOrEmpty(student.ImageFile))
                {
                    var image = await fileManager.Get(student.ImageFile);

                    if (image != null)
                    {
                        student.ImageLink = image.VirtualPath;
                    }
                }

                if (String.IsNullOrEmpty(student.ImageLink))
                {
                    student.ImageLink = student.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
                }

                if (string.IsNullOrEmpty(student.LastName))
                {
                    student.LastName = " ";
                }

                if (string.IsNullOrEmpty(student.Patronymic))
                {
                    student.Patronymic = " ";
                }

                student.thumb = student.ImageLink != null ? student.ImageLink.Replace("\\", "/") : "";
                student.Fio = String.Format("{0} {1} {2}.", student.Name, student.LastName, student.Patronymic);
                student.IsPassportValid = student.PassportState == ValidationState.Valid;
                student.IsStudentBookValid = student.StudentBookState == ValidationState.Valid;

            }

            return result.AsQueryable();
        }

        public async Task<IEnumerable<Student>> GetByName(string Name, bool exceptAdmins =false)
        {
            var nameArr = Name != null ? Name.Split(' ') : new string[] {string.Empty};

            var name = nameArr[0];
            var lastName = nameArr.Length > 1 ? nameArr[1] : null;
            var patronymic = nameArr.Length > 2 ? nameArr[2] : null;

            var students = await GetAll().Include(x=>x.University)
                     .Where(x=>x.Id != 3656)
                     .WhereIf(!string.IsNullOrEmpty(name) && lastName == null,
                        x =>
                            (x.Name.ToUpper().Contains(name.ToUpper()) || x.LastName.ToUpper().Contains(name.ToUpper()) ||
                            x.Patronymic.ToUpper().Contains(name.ToUpper()) || x.Phone.ToUpper().Contains(name.ToUpper()) || x.Email.ToUpper().Contains(name.ToUpper())))    
                     .Where(x=>!exceptAdmins  || x.Role != Role.Administrator)
                     .WhereIf(!string.IsNullOrEmpty(name) && lastName != null,
                        x =>
                            (x.Name.ToUpper().Contains(name.ToUpper())))
                    .WhereIf(lastName != null,
                        x =>
                            (x.LastName.ToUpper().Contains(lastName.ToUpper())))
                    .WhereIf(patronymic != null,
                        x =>
                            (x.Patronymic.ToUpper().Contains(patronymic.ToUpper())))
                            .OrderBy(x => x.Name)
                            .ToListAsync();

            foreach (var student in students)
            {
                if (!String.IsNullOrEmpty(student.ImageFile))
                {
                    var image = await fileManager.Get(student.ImageFile);

                    if (image != null)
                    {
                        student.ImageLink = image.VirtualPath;
                    }
                }

                if (String.IsNullOrEmpty(student.ImageLink))
                {
                    student.ImageLink = student.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
                }

                student.IsPassportValid = student.PassportState == ValidationState.Valid;
                student.IsStudentBookValid = student.StudentBookState == ValidationState.Valid;

                student.UniversityName = student.University != null ? student.University.Name : "";
                student.University = student.University != null ? new University { Id = student.University.Id } : null;
            }

            return students;
        }

        public async Task<IEnumerable<Student>> GetFiltered(StudentFilter search)
        {

            var nameArr = search.Name != null ? search.Name.Split(' ') : new string[]{string.Empty};

            var name = nameArr[0];
            var lastName = nameArr.Length > 1 ? nameArr[1] : null;
            var patronymic = nameArr.Length > 2 ? nameArr[2] : null;

            var grantId = search.GrantId ?? 0;

           long[] grantStudents = null;

            if (grantId != 0)
            {
                grantStudents = grantstudentRepo.GetAll()
                    .Where(x => x.GrantId == grantId)
                    .WhereIf(search.IsWinner.HasValue, x=>x.IsWinner == search.IsWinner.Value)
                    .Select(x => x.StudentId).ToArray();
            }

            int birthyear = 0;
            int birthmonth = 0;
            int birthday = 0;


            if (!search.Name.IsEmpty() && search.Name.Contains("."))
            {
                var daydata = search.Name.Split('.');

                if(daydata.Length == 3)
                {
                    Int32.TryParse(daydata[0], out birthday);
                    Int32.TryParse(daydata[1], out birthmonth);
                    Int32.TryParse(daydata[2], out birthyear);
                }
            }


            Citizenship citizenship = search.Citizenship.HasValue ? search.Citizenship.Value : Citizenship.NotSet;
            ValidationState pass = search.PersonalData.HasValue ? search.PersonalData.Value : ValidationState.NotFilled;
            ValidationState recordBook = search.RecordBook.HasValue
                ? search.RecordBook.Value
                : ValidationState.NotFilled;

            ValidationState income = search.Income.HasValue
              ? search.Income.Value
              : ValidationState.NotFilled;


            var now = DateTime.Now;

            var year = now.Year;
            var month = now.Month;
            var day = now.Day;


            var students = GetAll().Include(x => x.University).Include(x => x.PersonalInfo)
                .Where(x => x.Id != 3656 && x.Id != 151)
                .WhereIf(birthyear == 0 && !string.IsNullOrEmpty(name) && lastName == null,
                    x =>
                        (x.Name.ToUpper().Contains(name.ToUpper()) || x.LastName.ToUpper().Contains(name.ToUpper()) ||
                         x.Patronymic.ToUpper().Contains(name.ToUpper()) || x.Phone.ToUpper().Contains(name.ToUpper()) ||
                         x.Email.ToUpper().Contains(name.ToUpper())))
                .WhereIf(birthyear == 0 && !string.IsNullOrEmpty(name) && lastName != null,
                    x =>
                        (x.Name.ToUpper().Contains(name.ToUpper())))
                .WhereIf(birthyear == 0 && lastName != null,
                    x =>
                        (x.LastName.ToUpper().Contains(lastName.ToUpper())))
                .WhereIf(birthyear == 0 && patronymic != null,
                    x =>
                        (x.Patronymic.ToUpper().Contains(patronymic.ToUpper())))
                .WhereIf(grantStudents != null, x => grantStudents.Contains(x.Id))
                .WhereIf(search.Citizenship.HasValue, x => x.PersonalInfo.Citizenship == citizenship)
                .WhereIf(search.PersonalData.HasValue, x => x.PassportState == pass)
                .WhereIf(search.RecordBook.HasValue, x => x.StudentBookState == recordBook)
                .WhereIf(search.Income.HasValue, x => x.IncomeState == income)
                .WhereIf(search.UniversityId.HasValue, x => x.UniversityId == search.UniversityId.Value)
                .WhereIf(birthyear > 0, x => x.PersonalInfo.Birthday.Value.Year == birthyear)
                .WhereIf(birthmonth > 0, x => x.PersonalInfo.Birthday.Value.Month == birthmonth)
                .WhereIf(birthday > 0, x => x.PersonalInfo.Birthday.Value.Day == birthday)
                //.WhereIf(search.IsPassportOutDate.HasValue && search.IsPassportOutDate.Value, x=>
                //    ((x.PersonalInfo.Birthday.Value.Year - now.Year > 20) ||
                //    (x.PersonalInfo.Birthday.Value.Year - now.Year == 20 && x.PersonalInfo.Birthday.Value.Month > now.Month) ||
                //    (x.PersonalInfo.Birthday.Value.Year - now.Year == 20 && x.PersonalInfo.Birthday.Value.Month == now.Month && x.PersonalInfo.Birthday.Value.Day >= now.Day))
                //    && x.PersonalInfo.PassportIssueDate.Value.Year < x.PersonalInfo.Birthday.Value.Year + 20)
                .WhereIf(search.IsPassportOutDate.HasValue && search.IsPassportOutDate.Value, x => x.PersonalInfo.Birthday.HasValue && x.PersonalInfo.PassportIssueDate.HasValue &&
                    x.PersonalInfo.PassportIssueDate.Value.Year - x.PersonalInfo.Birthday.Value.Year < 20 &&
                    (year - x.PersonalInfo.Birthday.Value.Year > 20 ||
                        (year - x.PersonalInfo.Birthday.Value.Year == 20 &&
                            (month > x.PersonalInfo.Birthday.Value.Month || (month == x.PersonalInfo.Birthday.Value.Month && day >= x.PersonalInfo.Birthday.Value.Day))))

                    )
                .Where(x => x.Id != 34);
              //   .WhereIf(search.lastId != 0, x=>x.Id < search.lastId);

            switch (search.sortBy)
            {
                case 0:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.Id).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.Id).Skip(search.skip).Take(50);
                    }
                    break;

                case 1:
                    
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.Name).ThenBy(x => x.LastName).ThenBy(x => x.Patronymic).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.Name).ThenByDescending(x => x.LastName).ThenByDescending(x => x.Patronymic).Skip(search.skip).Take(50);
                    }
                    
                    break;

                case 2:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.Score).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.Score).Skip(search.skip).Take(50);
                    }
                    break;

                case 3:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.University.Name).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.University.Name).Skip(search.skip).Take(50);
                    }
                    break;

                case 4:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.PassportState).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.PassportState).Skip(search.skip).Take(50);
                    }
                    break;

                case 5:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.StudentBookState).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.StudentBookState).Skip(search.skip).Take(50);
                    }
                    break;

                case 6:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.Phone).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.Phone).Skip(search.skip).Take(50);
                    }
                    break;

                case 7:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.Email).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.Email).Skip(search.skip).Take(50);
                    }
                    break;

                case 8:
                    if (!search.Asc)
                    {
                        students = students.OrderBy(x => x.EditDate).Skip(search.skip).Take(50);
                    }
                    else
                    {
                        students = students.OrderByDescending(x => x.EditDate).Skip(search.skip).Take(50);
                    }
                    break;
            }



            var result = new List<Student>();

            try
            {
                result = await students.ToListAsync();
            }
            catch (Exception ex)
            {
                var test = ex.Message;
            }

            



              /*    .OrderByDescending(x=>x.Id)
                    .Take(20)
                    .ToListAsync(); */

            foreach (var student in result)
            {
                student.IsPassportValid = student.PassportState == ValidationState.Valid;
                student.IsStudentBookValid = student.StudentBookState == ValidationState.Valid;

                student.UniversityName = student.University != null ? student.University.Name : "";
                student.University = student.University != null ? new University {Id = student.University.Id} : null;
                student.Citizenship = student.PersonalInfo != null
                    ? student.PersonalInfo.Citizenship
                    : Citizenship.NotSet;

                student.PersonalInfo = null;

                //student.EditDate = student.ProfileEditDate > student.PersonalDataEditDate ? student.ProfileEditDate : student.PersonalDataEditDate;
                //if(student.RecordBookEditDate > student.EditDate)
                //{
                //    student.EditDate = student.RecordBookEditDate;
                //}

                //if (student.EditDate.Year > 2000)
                //{
                //    student.EditDateTime = string.Format("{0} {1}",
                //        student.EditDate.ToString("dd.MM.yyyy"), student.EditDate.ToString("HH:mm:ss"));
                //}
                //else
                //{
                //    student.EditDateTime = "";
                //}

              
            }

            return students;
        }

        public async Task<DataResult> GetUserStat(long grantId)
        {
            var grantStudent = await grantstudentRepo.GetAll().Where(x => x.GrantId == grantId).Select(x=>x.StudentId).ToArrayAsync();


            var allCount = await GetAll()
                .WhereIf(grantId > 0, x=> grantStudent.Contains(x.Id))
                .CountAsync();

            var passportNotFilledCount = await GetAll().Include(x => x.PersonalInfo)
                 .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                 .Where(x => x.PassportState == ValidationState.NotFilled)
                 .CountAsync();

            var studentBookNotFilled = await GetAll()
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                .Where(x => x.StudentBookState == ValidationState.NotFilled)
                .CountAsync();


            var validPassportCount = await GetAll().Include(x=>x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                .Where(x=>x.PassportState == ValidationState.Valid)
                .CountAsync();

            var validStudentBookCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                .Where(x => x.StudentBookState == ValidationState.Valid)
                .CountAsync();


            var fullValidCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                .Where(x => x.PassportState == ValidationState.Valid)
                .Where(x => x.StudentBookState == ValidationState.Valid)
                .CountAsync();


            var invalidPassportCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
               .Where(x => x.PassportState == ValidationState.Invalid)
               .CountAsync();

            var invalidStudentBookCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                .Where(x => x.StudentBookState == ValidationState.Invalid)
                .CountAsync();


            var editedPassportCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
               .Where(x => x.PassportState == ValidationState.Unknown)
               .Where(x => x.PassValidationComment == null || x.PassValidationComment.Length == 0)
               .CountAsync();

            var editedStudentBookCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                 .Where(x => x.BookValidationComment == null || x.BookValidationComment.Length == 0)
                .Where(x => x.StudentBookState == ValidationState.Unknown)
                .CountAsync();


            var editedPassportCountWaitCheck = await GetAll().Include(x => x.PersonalInfo)
             .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
             .Where(x => x.PassportState == ValidationState.Unknown)
             .Where(x => x.PassValidationComment != null && x.PassValidationComment.Length > 0)
             .CountAsync();

            var editedStudentBookCountWaitCheck = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
                 .Where(x => x.BookValidationComment != null && x.BookValidationComment.Length > 0)
                .Where(x => x.StudentBookState == ValidationState.Unknown)
                .CountAsync();


            var invalidPassporOrBookCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
               .Where(x => x.PassportState == ValidationState.Invalid || x.StudentBookState == ValidationState.Invalid)
               .CountAsync();

            var notCheckedPassportCount = await GetAll().Include(x => x.PersonalInfo)
                .WhereIf(grantId > 0, x => grantStudent.Contains(x.Id))
               .Where(x =>  x.PassportState == ValidationState.Unknown || x.StudentBookState == ValidationState.Unknown)
               .Where(x => x.PassportState != ValidationState.Invalid && x.StudentBookState != ValidationState.Invalid)
                .CountAsync();

            var result = new
            {
                allCount,
                passportNotFilledCount,
                studentBookNotFilled,
                validPassportCount,
                validStudentBookCount,
                fullValidCount,
                invalidPassportCount,
                invalidStudentBookCount,
                editedPassportCount,
                editedStudentBookCount,
                editedPassportCountWaitCheck,
                editedStudentBookCountWaitCheck,
                invalidPassporOrBookCount,
                notCheckedPassportCount

            };

            return DataResult.Ok(result);

        }

        public async Task<DataResult> SetIncomeValidState(long id)
        {
            var student = await Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await GetCurrent();

            if (student.IncomeState != ValidationState.Valid)
            {
                var rec = new ValidationHistory
                {
                    ValidationUserId = id,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Valid,
                    Target = ValidationTarget.Income
                };

                await validationHistRepo.Create(rec);
            }

            student.IncomeState = ValidationState.Valid;
            student.IncomeValidationComment = null;

            await Update(student);

            return DataResult.Ok();
        }

        public async Task<DataResult> SetPassportValidState(long id)
        {
            var student = await Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await GetCurrent();

            if (student.PassportState != ValidationState.Valid)
            {
                var rec = new ValidationHistory
                {
                    ValidationUserId = id,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Valid,
                    Target = ValidationTarget.PersonalInfo
                };

                await validationHistRepo.Create(rec);
            }

            student.PassportState = ValidationState.Valid;
            student.PassValidationComment = null;

            await Update(student);

            return DataResult.Ok();
        }

        public async Task<DataResult> SetIncome(long id, int income)
        {
            var student = await Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            student.Income = income;

            student.IncomeState = ValidationState.Unknown;
            student.IncomeEditDate = DateTime.Now;
            await Update(student);

            return DataResult.Ok(student);
        }


        public async Task<DataResult> SetBookValidState(long id)
        {
            var student = await Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await GetCurrent();

            if (student.StudentBookState != ValidationState.Valid)
            {
                var rec = new ValidationHistory
                {
                    ValidationUserId = id,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Valid,
                    Target = ValidationTarget.RecordBook
                };

                await validationHistRepo.Create(rec);
            }

            student.StudentBookState = ValidationState.Valid;
            student.BookValidationComment = null;

            await Update(student);

            return DataResult.Ok();
        }


        public async override Task<IQueryable<Student>> GetAllAsync()
        {
            var result = await GetAll().Include(x=>x.University).Where(x=>x.Name.Length>0).OrderBy(x=>x.Name).ToListAsync();

            foreach (var student in result)
            {
                student.IsPassportValid = student.PassportState == ValidationState.Valid;
                student.IsStudentBookValid = student.StudentBookState == ValidationState.Valid;

                student.UniversityName = student.University != null ? student.University.Name : "";
                student.University = student.University != null ? new University {Id = student.University.Id} : null;

            }

            return result.AsQueryable();
        }

        public async Task<DataResult> IsProfileFilled(long id)
        {
            var data = await GetAll()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();


            if (string.IsNullOrEmpty(data.Name) ||
                string.IsNullOrEmpty(data.LastName) ||
                string.IsNullOrEmpty(data.Patronymic) ||
                !data.UniversityId.HasValue)
            {
                return DataResult.Ok(false);
            }

            return DataResult.Ok(true);
        }


        public async Task<DataResult> IsIncomeFilled(long id)
        {
            var data = await GetAll()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            var minDate = new DateTime(2019, 1, 1);

            if (data.IncomeState == ValidationState.NotFilled)
            {
                return DataResult.Ok(false);
            }

            if (string.IsNullOrEmpty(data.IncomeFiles) || data.IncomeFiles.Replace("|", "").Length == 0 || data.IncomeEditDate < minDate)
            {
                return DataResult.Ok(false);
            }

            var arr = data.IncomeFiles.Split('|');

            int count = 0;
            foreach (string str in arr)
            {
                if (!string.IsNullOrEmpty(str) && str.Trim().Length > 0)
                {
                    count++;
                }
            }

            if (count < 1)
            {
                return DataResult.Ok(false);
            }

            if (!data.Income.HasValue)
            {
                return DataResult.Ok(false);
            }

            return DataResult.Ok(true);
        }
       

        public async Task<DataResult> IsRecordBookFilled(long id)
        {
            var data = await GetAll()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

            var minDate = new DateTime(2019, 1, 1);

            if (data.StudentBookState == ValidationState.NotFilled)
            {
                return DataResult.Ok(false);
            }

            if (string.IsNullOrEmpty(data.RecordBookFiles) || data.RecordBookFiles.Replace("|", "").Length == 0 || data.RecordBookEditDate < minDate) 
            {
                return DataResult.Ok(false);
            }

            var arr = data.RecordBookFiles.Split('|');

            int count = 0;
            foreach (string str in arr)
            {
                if (!string.IsNullOrEmpty(str) && str.Trim().Length > 0)
                {
                    count++;
                }
            }

            if (count < 2)
            {
                return DataResult.Ok(false);
            }

            return DataResult.Ok(true);
        }
        public async Task<DataResult> GetValidationStat(EventFilter filter)
        {
            if (filter.After.HasValue)
            {
                filter.After = filter.After.Value.Date;
            }

            if (filter.Before.HasValue)
            {
                filter.Before = filter.Before.Value.Date;
            }


            var result = await validationHistRepo.GetAll()
                 .Where(x=>x.ModeratorId != 34)
                .WhereIf(filter.After.HasValue, x=>x.EditDate >= filter.After)
                .WhereIf(filter.Before.HasValue, x => x.EditDate <= filter.Before)
                .Include(x => x.Moderator)
                .GroupBy(x => x.Moderator)
                .Select(x => new
                {
                    x.Key.Name,
                    x.Key.LastName,
                    x.Key.Patronymic,
                    ValidationCount = x.Select(y=>y.ValidationUserId).Distinct().Count()
                }).ToListAsync();

            return DataResult.Ok(result);
        }


        private async Task<string> getAvatar(string imageId, Sex pol ){

            var image = !string.IsNullOrEmpty(imageId) ? await fileManager.Get(imageId) : null;

             if (image != null)
             {
                return image.VirtualPath;
             }
                
             return pol == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
        }


         public async Task<DataResult> GetValidationHistory(ValidationTarget target, long studentId)
         {
            var data = await validationHistRepo.GetAll()
              .Where(x => x.ValidationUserId == studentId && x.Target == target)
              .Include(x => x.Moderator)
              .OrderByDescending(x => x.EditDate)
              .ToListAsync();

         
            if (data.Any())
            {
                var result = data.Select(async (x) => new
                {
                    name = String.Format("{0} {1} {2}", x.Moderator.Name, x.Moderator.LastName, x.Moderator.Patronymic),
                    messages = new string[] { string.Format("{0} {1} : {2}",
                        x.EditDate.ToString("dd.MM.yyyy"),
                        x.EditDate.ToString("HH:mm:ss"),
                        (x.State == ValidationState.Valid ? "Данные верны" : (!string.IsNullOrEmpty(x.ValidationMessage) ? x.ValidationMessage : "Есть замечания"))) },
                    image = await getAvatar(x.Moderator.ImageFile, x.Moderator.Sex),
                    isValid = x.State == ValidationState.Valid,
                    userId = x.ModeratorId
                })
                .Select(x => x.Result).ToArray();
              
                return DataResult.Ok(result);
            }

            return DataResult.Ok(null);


        }

        public async Task<DataResult> GetValidatorName(ValidationTarget target, long studentId)
        {
            var result = await validationHistRepo.GetAll()
                .Where(x => x.ValidationUserId == studentId && x.Target == target)
                .Include(x => x.Moderator)
                .OrderByDescending(x => x.EditDate)
                .ToListAsync();

            var res = new StringBuilder();

            if (result.Any())
            {

                foreach (var valRec in result)
                {
                    if (valRec.Moderator != null)
                    {
                        res.AppendFormat(" {3} {4} :  {0} {1} {2}  - {5} {6} {7}", valRec.Moderator.LastName, valRec.Moderator.Name,
                            valRec.Moderator.Patronymic, valRec.EditDate.ToString("dd.MM.yyyy"),
                            valRec.EditDate.ToString("HH:mm:ss"),
                            valRec.State == ValidationState.Valid ? "Данные верны" : "Есть замечания", valRec.ValidationMessage, "<br>");
                    }
                }

                return DataResult.Ok(res.ToString());
            }

            return DataResult.Ok("");

        }

        public async Task<DataResult> GetContactInfo()
        {
            var result = new ContactInfo();

            var curStud = await GetCurrent();

            if (curStud.UniversityId != null)
            {
                var univerInfo = await universityService.GetAll()
                .Include(x => x.Curator)
                .Where(x => x.Id == curStud.UniversityId).Select(x => new
                {
                    UniversityName = x.Name,
                    x.CuratorId,
                    x.City
                }).FirstOrDefaultAsync();

                if (univerInfo != null)
                {
                    result.UniversityName = univerInfo.UniversityName;
                  

                    if (univerInfo.City == "Набережные Челны")
                    {
                        result.ZoneName = "Набережночелнинская";
                        result.ZoneManName = "Назиля Габдрахманова";
                        result.ZoneManPhone = "+7 (951) 898-47-82";
                    }
                    else if (univerInfo.City == "Альметьевск")
                    {
                        result.ZoneName = "Альметьевская";
                        result.ZoneManName = "Гульназ Нургалиева";
                        result.ZoneManPhone = "+7 (919) 648-69-18";
                    }
                    else if (univerInfo.City == "Чистополь")
                    {
                        result.ZoneName = "Чистопольская";
                        result.ZoneManName = "Эльза Антонова";
                        result.ZoneManPhone = "+7 (905) 313-40-75";
                    }


                    if (univerInfo.CuratorId != null)
                    {
                        var curatorInfo = await GetAll().Where(x => x.Id == univerInfo.CuratorId).FirstOrDefaultAsync();


                        if (curatorInfo != null)
                        {
                            result.CuratorName = curatorInfo.Name + " " + curatorInfo.LastName + " " +
                                                 curatorInfo.Patronymic;
                            result.CuratorPhone = !string.IsNullOrEmpty(curatorInfo.Phone)
                                ? "+7" + curatorInfo.Phone
                                : "";
                        }
                    }
                }

               
            }

            return DataResult.Ok(result);
        }
    }
}
