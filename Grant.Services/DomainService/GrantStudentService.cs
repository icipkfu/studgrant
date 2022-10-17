using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Grant.Core.Config;
using Grant.Core.Context;
using Grant.Core.DbContext;
using Grant.Core.Notification;
using Grant.Core.Reports;
using Grant.Reports.Reports;
using Grant.Utils.Extensions;
using Ionic.Zip;
using LightInject;
using Grant.Utils.Extensions;


namespace Grant.Services.DomainService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enum;
    using Core;
    using Core.Entities;
    using DataAccess;

    public class GrantStudentService : BaseDomainService<GrantStudent>, IGrantStudentService
    {
        private readonly IStudentService _studentService;
        private readonly IRepository<Student> _studentrepo;
        private readonly IUniversityService _universityService;
        private readonly IFileManager _fileManager;
        private readonly IGrantEventService _grantEventService;
        private readonly IGrantService _grantService;
        private readonly IDbfReportGenerator _dbfReportGenerator;
        private IRepository<GrantQuota> _grantQuotaRepo;
        private IRepository<GrantStudent> _grantUniversityRepo;
        private readonly IDomainService<PersonalInfo> personalInfoService;
        private readonly IDomainService<GrantFileInfo> fileService;
        private readonly IRepository<ValidationHistory> validationHistRepo;
        private readonly INotificationQueueProvider _notificator;
        private readonly IRepository<Achievement> achRepository;

        private GrantDbContext db = new GrantDbContext();

        private readonly IConfigProvider _configProvider;
        private string _storagePath;
        private string _virtualPath;
        

        protected IServiceContainer Container
        {
            get
            {
                return ApplicationContext.Current.Container;
            }
        }

        protected string StoragePath
        {
            get
            {
                return _storagePath ??
                (_storagePath = _configProvider.GetConfig().GetAs<string>("FileStorage", "ReportPath"));
            }
        }

        public GrantStudentService(IRepository<GrantStudent> repository,
                                IStudentService studentService, 
                                 IUniversityService universityService,
                                 IFileManager fileManager, 
                                 IGrantEventService grantEventService, 
                                 IGrantService grantService, 
                                 IDbfReportGenerator dbfReportGenerator,
                                 IRepository<GrantQuota> grantQuotaRepo, 
                                 IConfigProvider configProvider, 
                                 IRepository<Student> studentrepo, 
                                 IDomainService<PersonalInfo> domain,
                                 IDomainService<GrantFileInfo> fileService,
                                 IRepository<ValidationHistory> validationHistRepo,
                                 INotificationQueueProvider notificator,
                                 IRepository<GrantStudent> grantUniversityRepo,
                                 IRepository<Achievement> achRepository)
            : base(repository)
        {
            _studentService = studentService;
            _universityService = universityService;
            _fileManager = fileManager;
            _grantEventService = grantEventService;
            _grantService = grantService;
            _dbfReportGenerator = dbfReportGenerator;
            _grantQuotaRepo = grantQuotaRepo;
            _configProvider = configProvider;
            _studentrepo = studentrepo;
            personalInfoService = domain;
            this.fileService = fileService;
            this.validationHistRepo = validationHistRepo;
            this.achRepository = achRepository;
            _notificator = notificator;
            _grantUniversityRepo = grantUniversityRepo;
        }


        public async Task<IEnumerable<University>> GetGrantUnivers(long grantId, bool additional)
        {
            var roleInfo = await _studentService.GetCurrentRole();

            var isAdmin = roleInfo.GrantsAdmin.Contains(grantId) || roleInfo.Role == Role.Administrator;

            var result = await GetAll().Include(x=>x.University)
                .Where(x => x.GrantId == grantId)
                .Where(x => x.IsWinner == false || additional == false)
                .Where(x => isAdmin || roleInfo.UniversCurator.Contains(x.UniversityId))
                .Select(x=>x.University).Distinct()
                .OrderBy(x=>x.Name)
                .ToListAsync(); 


            return result;
        }

        public async Task<DataResult> GetGrantStudents(long grantId, long universityId, GrantStudentFilter filter)
        {

            var guids = await db.GrantStudents.Include(x => x.Student)
                .Where(x => x.GrantId == grantId && x.UniversityId == universityId)
                .Where(x=>x.Student.ImageFile != null)
                .Select(x => x.Student.ImageFile)
                .ToArrayAsync();

            var fileDict =  await db.FilesInfo.Where(x => guids.Contains(x.Guid))
                .GroupBy(x => x.Guid)
                .ToDictionaryAsync(x => x.Key, y => y.Select(z => z.VirtualPath).First());

            //public ValidationState? selectedScore { get; set; }

            //public ValidationState? selectedIncome { get; set; }

            var recordBookState = filter.selectedRecordbook.HasValue ? filter.selectedRecordbook.Value : ValidationState.All;
            var incomeState = filter.selectedIncome.HasValue ? filter.selectedIncome.Value : ValidationState.All;

            var year = DateTime.Now.Year - 1;

            var data = await db.GrantStudents
                     .WhereIf(filter.social, x => x.IsSocialHelp)
                     .WhereIf(filter.active, x => x.IsSocialActive)
                     .WhereIf(filter.selectedRecordbook.HasValue, x=>x.Student.StudentBookState == recordBookState)
                     .WhereIf(filter.selectedIncome.HasValue && incomeState != ValidationState.All, x=>x.Student.IncomeState == incomeState)
                     .Where(x => x.GrantId == grantId && x.UniversityId == universityId)
                     .Include(x => x.Student)
                     .Include(x => x.University)
                     .Include(x => x.Student.PersonalInfo)
                     .ToListAsync();


            var studentIds = data.Select(x => x.StudentId).ToArray();

            if (filter.selectedAchievements.HasValue && filter.selectedAchievements.Value != ValidationState.All)
            {
                var achievementState = filter.selectedAchievements.Value;

                var validAchievementsStudentsIdArr = await achRepository.GetAll()
                    .Where(x => x.ValidationState == achievementState)
                    .Where(x => studentIds.Contains(x.StudentId))
                    .Select(x => x.StudentId)
                    .Distinct()
                    .ToArrayAsync();

                data = data
                    .Where(x => validAchievementsStudentsIdArr.Contains(x.StudentId))
                    .ToList();
            }

            var achievementDict = (await db.Achievements
                                    .Where(x => studentIds.Contains(x.StudentId))
                                    .ToListAsync())
                                    .GroupBy(x => x.StudentId)
                                    .ToDictionary(x => x.Key, y =>  new
                                    {
                                        HasValidAchievements = y.Any(z=>z.ValidationState == ValidationState.Valid && z.Year >= year),
                                        HasInvalidAchievements = y.Any(z => z.ValidationState == ValidationState.Invalid && z.Year >= year),
                                        HasNotCheckedAchievements = y.Any(z => z.ValidationState == ValidationState.Unknown && z.Year >= year),
                                        CorrectScore = y.Where(z=>z.ValidationState == ValidationState.Valid).SafeSum(r=> r.Score)
                                    });

            var result = data.Select(x => new
            {
                Fio = x.Student.Name + " " + x.Student.LastName + " " + x.Student.Patronymic,
                Score = filter.selectedScore.HasValue && filter.selectedScore.Value == ValidationState.Valid ?
                           (achievementDict.ContainsKey(x.StudentId) ? achievementDict[x.StudentId].CorrectScore : 0) : x.Student.Score,
                x.Student.Income,
                IsPassportValid = x.Student.PassportState == ValidationState.Valid,
                IsStudentBookValid = x.Student.StudentBookState == ValidationState.Valid,
                StudentBookStateName = ValidationStateName(x.Student.StudentBookState),
                IncomeStateName = ValidationStateName(x.Student.IncomeState),
                IsIncomeCorrect = x.Student.IncomeState == ValidationState.Valid,
                HasValidAchievements = achievementDict.ContainsKey(x.StudentId) ? achievementDict[x.StudentId].HasValidAchievements : false,
                HasInvalidAchievements = achievementDict.ContainsKey(x.StudentId) ? achievementDict[x.StudentId].HasInvalidAchievements : false,
                HasNotCheckedAchievements = achievementDict.ContainsKey(x.StudentId) ? achievementDict[x.StudentId].HasNotCheckedAchievements : false,
                UniversityName = x.University.Name,
                thumb = x.Student.ImageFile != null && fileDict.ContainsKey(x.Student.ImageFile) ? fileDict[x.Student.ImageFile]
                            : (x.Student.PersonalInfo.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png"),
                x.IsWinner,
                x.IsAdditionalWinner,
                Link = "<a target='_blank' href='/my#/user/page/" + x.StudentId + "'>Просмотр</a>",
                x.StudentId
            } )
                   .OrderByDescending(x => x.Score)
                .ToList();

            return DataResult.Ok(result);
        }

        public string ValidationStateName(ValidationState val)
        {
            switch (val)
            {
                case ValidationState.Unknown: return "Не проверены";
                case ValidationState.Invalid: return "Есть замечания";
                case ValidationState.NotFilled: return "Не заполнено";
                case ValidationState.Valid: return "Данные верны";
            }

            return "";
        }



        public async Task<DataResult> GetAdditionalStudents(AdditionalWinnerFilter filter)
        {

            long[] studIds = (await db.GrantStudents
                    .Where(x => x.GrantId == filter.GrantId)
                    .WhereIf(filter.UniversityId != 0, x => x.UniversityId == filter.UniversityId)
                    .Where(x => x.IsWinner == false)
                    .Include(x => x.Student)
                    .Select(x => new
                      {
                         x.StudentId,
                         x.Student.Score
                     })
                    .OrderByDescending(x => x.Score)
                    .Skip(filter.skip).Take(50)
                    .ToArrayAsync()).Select(x=>x.StudentId).ToList().ToArray();

            var guids = await db.GrantStudents.Include(x => x.Student)
               .Where(x => x.GrantId == filter.GrantId)
               .Where(x => studIds.Contains(x.StudentId))
               .WhereIf(filter.UniversityId != 0, x => x.UniversityId == filter.UniversityId)
               .Where(x => x.Student.ImageFile != null)
               .Select(x => x.Student.ImageFile)
               .ToArrayAsync();

            var fileDict = await db.FilesInfo.Where(x => guids.Contains(x.Guid))
                .GroupBy(x => x.Guid)
                .ToDictionaryAsync(x => x.Key, y => y.Select(z => z.VirtualPath).First());


            var result = db.GrantStudents
                     .Where(x => x.GrantId == filter.GrantId)
                     .WhereIf(filter.UniversityId != 0, x => x.UniversityId == filter.UniversityId)
                     .Include(x => x.Student)
                     .Include(x => x.University)
                     .Where(x => x.IsWinner == false);

             switch (filter.sortBy)
            {
                case 2:
                    if (!filter.Asc)
                    {
                        result = result.OrderBy(x => x.Student.Name).ThenBy(x => x.Student.LastName).ThenBy(x => x.Student.Patronymic).Skip(filter.skip).Take(50);
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.Student.Name).ThenByDescending(x => x.Student.LastName).ThenByDescending(x => x.Student.Patronymic).Skip(filter.skip).Take(50);
                    }
                    break;

                case 3:
                    
                    if (!filter.Asc)
                    {
                        result = result.OrderBy(x => x.Student.Score).Skip(filter.skip).Take(50);
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.Student.Score).Skip(filter.skip).Take(50);
                    }
                    
                    break;

                 default:
                       
                    if (!filter.Asc)
                    {
                        result = result.OrderBy(x => x.Student.Score).Skip(filter.skip).Take(50);
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.Student.Score).Skip(filter.skip).Take(50);
                    }
                    
                    break;
             }

            var students = (await result
                     .ToListAsync())
                      .Select(x => new
                      {
                          Fio = x.Student.Name + " " + x.Student.LastName + " " + x.Student.Patronymic,
                          x.Student.Score,
                          IsPassportValid = x.Student.PassportState == ValidationState.Valid,
                          IsStudentBookValid = x.Student.StudentBookState == ValidationState.Valid,
                          UniversityName = x.University.Name,
                          thumb = x.Student.ImageFile != null && fileDict.ContainsKey(x.Student.ImageFile) ? fileDict[x.Student.ImageFile] : "",
                          x.IsWinner,
                          x.IsAdditionalWinner,
                          Link = "<a target='_blank' href='/my#/user/page/" + x.StudentId + "'>Просмотр</a>",
                          x.StudentId
                      })
                     
                .ToList();

            return DataResult.Ok(students);
        }

        public async Task<DataResult> ChooseWinner(long grantId, long studentId)
        {
            var record =
                await GetAll().Where(x => x.GrantId == grantId && x.StudentId == studentId).SingleOrDefaultAsync();

            if (record == null)
            {
                return DataResult.Failure("Нет такого участника в данном гранте");
            }

            var winnerCount =
                await
                    GetAll()
                        .Where(
                            x =>
                                x.GrantId == grantId && (x.IsWinner || x.IsAdditionalWinner) &&
                                x.UniversityId == record.UniversityId)
                        .CountAsync();

            var quota =
                await
                    _grantQuotaRepo.GetAll()
                        .Where(x => x.GrantId == grantId && x.UniversityId == record.UniversityId)
                        .SingleOrDefaultAsync();

            if (quota != null)
            {
                quota.Quota -= winnerCount;
            }

            if (quota != null && quota.Quota > 0)
            {
                record.IsWinner = true;

                await Update(record);

                var grant = await _grantService.Get(grantId);
                var stud = await _studentService.Get(studentId);

                var eventText = String.Format("{0} {1} {2} выбран в победители гранта",
                    stud.Name,
                    stud.LastName,
                    stud.Patronymic);

                await
                    _grantEventService.CreateEvent(grant, "Выбран победитель гранта", eventText,
                        EventType.GrantWinnerSelected);
            }
            else
            {
                return DataResult.Failure("Квота исчерпана");
            }
          

            return DataResult.Ok();
        }

        public async Task<DataResult> CancelWinner(long grantId, long studentId)
        {
            var record =
                await GetAll().Where(x => x.GrantId == grantId && x.StudentId == studentId).SingleOrDefaultAsync();

            if (record == null)
            {
                return DataResult.Failure("Нет такого участника в данном гранте");
            }

            record.IsWinner = false;

            await Update(record);

            var grant = await _grantService.Get(grantId);
            var stud = await _studentService.Get(studentId);

            var curator = await _studentService.GetCurrent();

            var eventText = String.Format("{0} {1} {2} отменил выбор {3} {4} {5} в качестве победителя",
                curator.Name,
                curator.LastName,
                curator.Patronymic,
                stud.Name,
                stud.LastName,
                stud.Patronymic);

            await _grantEventService.CreateEvent(grant, "Отмена выбора победителя", eventText, EventType.GrantWinnerCanceled);

            return DataResult.Ok();
        }

        public async Task<DataResult> ChooseAdditionalWinner(long grantId, long studentId)
        {
            var record = await GetAll().Where(x => x.GrantId == grantId && x.StudentId == studentId && x.IsWinner == false).SingleOrDefaultAsync();

            if (record == null)
            {
                return DataResult.Failure("Нет такого участника в данном гранте");
            }

            var allWinnerCount =
               await
                   GetAll()
                       .Where(
                           x =>
                               x.GrantId == grantId && (x.IsWinner || x.IsAdditionalWinner))
                       .CountAsync();

            var grant = await _grantService.Get(grantId);

            var quota = grant.FullQuota - allWinnerCount;

            if (quota > 0)
            {
                quota -= 1;

                record.IsAdditionalWinner = true;

                await Update(record);
                var stud = await _studentService.Get(studentId);

                var eventText = String.Format("{0} {1} {2} выбран дополнительным победителем гранта",
                    stud.Name,
                    stud.LastName,
                    stud.Patronymic);

                await
                    _grantEventService.CreateEvent(grant, "Выбран дополнительный победитель гранта", eventText,
                        EventType.GrantAdditionalWinnerSelected);


                return DataResult.Ok(String.Format("Выбран дополнительный победитель гранта, остаток квоты: {0}",quota));
            }
            else
            {
                return DataResult.Failure("Квота исчерпана");
            }
        }

        public async Task<DataResult> CancelAdditionalWinner(long grantId, long studentId)
        {
            var record =
              await GetAll().Where(x => x.GrantId == grantId && x.StudentId == studentId && x.IsAdditionalWinner).SingleOrDefaultAsync();

            if (record == null)
            {
                return DataResult.Failure("Нет такого участника в данном гранте");
            }

            record.IsAdditionalWinner = false;

            await Update(record);

            var grant = await _grantService.Get(grantId);
            var stud = await _studentService.Get(studentId);

            var curator = await _studentService.GetCurrent();

            var eventText = String.Format("{0} {1} {2} отменил выбор {3} {4} {5} в качестве дополнительного победителя",
                curator.Name,
                curator.LastName,
                curator.Patronymic,
                stud.Name,
                stud.LastName,
                stud.Patronymic);

            await _grantEventService.CreateEvent(grant, "Отмена выбора дополнительного победителя", eventText, EventType.GrantAdditionalWinnerCanceled);

            return DataResult.Ok();
        }

        public async Task<DataResult> DeclineGrant(long grantId)
        {
             var grant = await _grantService.Get(grantId);
              var stud = await _studentService.GetCurrent();

              var grantStudent = await
                 GetAll().Where(x => x.GrantId == grantId && x.StudentId == stud.Id).SingleOrDefaultAsync();

              if (grantStudent == null)
              {
                  return DataResult.Failure("Вы не являетесь участником данного гранта");
              }

             await Delete(grantStudent.Id);

            var eventText = string.Format("«{0} {1} {2} отказался от участия в '{3}'»", stud.Name, stud.LastName, stud.Patronymic, grant.Name);

            await _grantEventService.CreateEvent(grant, "Отказ от участия", eventText, EventType.GrantUserCancel);

            return DataResult.Ok();
        }

        public async Task<DataResult> AcceptGrant(long grantId, bool isSocialActive, bool isSocialHelp)
        {

            if(isSocialHelp == isSocialActive)
            {
                throw new Exception("Зарегистрироваться можно либо как социально активный студент, либо для социалььной помощи");
            }

            var grant = await _grantService.Get(grantId);
            var stud = await _studentService.GetCurrent();

            if (!stud.UniversityId.HasValue)
                throw new Exception("Не заполнена информация о ВУЗе");

            var grantStudent = await
                GetAll().Where(x => x.GrantId == grantId && x.StudentId == stud.Id).SingleOrDefaultAsync();

            if (grant.Status != GrantStatus.Registration && stud.Id != 34)
            {
                return DataResult.Failure("Регистрация в конкурсе закрыта");
            }

            //if (DateTime.Now.Day > 25) //TODO
            //{
            //    return DataResult.Failure("Регистрация в конкурсе закрыта");
            //}

            // зачетка
             
            var bookFilled = await _studentService.IsRecordBookFilled(stud.Id);

            if (!(bool)bookFilled.Data)
            {
                throw new Exception("Не заполнена зачетка");
            }

            //справка о доходах
            if (isSocialHelp)
            {
                var incomeFilled = await _studentService.IsIncomeFilled(stud.Id);

                if (! (bool)incomeFilled.Data)
                {
                    throw new Exception("Не заполнена справка о доходах");
                }
            }

            if (grantStudent == null)
            {
                 grantStudent = new GrantStudent
                {
                    GrantId = grantId,
                    StudentId = stud.Id,
                    UniversityId = stud.UniversityId.Value,
                    IsSocialActive = isSocialActive,
                    IsSocialHelp = isSocialHelp
                };
            }
            else
            {
                return DataResult.Failure("Вы уже являетесь участником данного гранта");
            }

            await Create(grantStudent);

            var status = "Социальная помощь";

            if (isSocialActive)
            {
                status = "Социально активный";
            }

            //  var eventText = string.Format("«{0} {1} {2} стал участником конкурса '{3}'»", stud.Name, stud.LastName, stud.Patronymic, grant.Name, status);

            var eventText = string.Format("«{0} {1} {2} стал участником конкурса '{3}'»", stud.Name, stud.LastName, stud.Patronymic, grant.Name);

            await _grantEventService.CreateEvent(grant, "Регистрация участника", eventText, EventType.GrantUserRegister);

            return DataResult.Ok(grantStudent);
        }


        public async Task<Boolean> IsParticipant(long grantId)
        {
            var curId = await _studentService.GetCurrent();
            var result =  await GetAll().AnyAsync(x => x.StudentId == curId.Id && x.GrantId == grantId);

            return result;
        }

        public async Task<DataResult> GetWinnersList(long grantid, bool? onlyNewWinners)
        {
            var oldWinners = await
                GetAll()
               .Include(x => x.Student)
               .Where(x => x.GrantId != grantid)
               .Where(x => x.IsWinner || x.IsAdditionalWinner)
               .Select(x => x.StudentId).ToArrayAsync();

            var studentDict = await _studentService.GetAll().GroupBy(x => x.Id).ToDictionaryAsync(z => z.Key, y => y.First());

            var mainUniverList = await _grantQuotaRepo.GetAll()
               .Include(x => x.University)
               .Where(x => x.GrantId == grantid)
               .Where(x => x.WinnerReport != null && x.WinnerReport.Length > 1)
               .Select(x => new
               {
                   x.UniversityId,
                   x.University.Name
               }).OrderBy(x => x.Name).ToListAsync();


            var mainIds = mainUniverList.Select(x => x.UniversityId).ToArray();

            var additionalUniverList = 
                (await _grantUniversityRepo.GetAll()
                .Include(x=>x.Student)
                .Include(x=>x.Student.University)
                // .Include(x => x.University)
               .Where(x => x.GrantId == grantid && x.IsAdditionalWinner)
               //.Where(x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
               .Select(x => new
               {
                   x.UniversityId,
                   x.University.Name
               })
               .OrderBy(x => x.Name).ToListAsync())
               .Distinct(x => x.UniversityId)
               .ToList();

            var addIds = additionalUniverList.Distinct(x =>x.UniversityId).Select(x => x.UniversityId).ToArray();



            var winnerDict = await GetAll()
                .Include(x => x.Student)
                .WhereIf(onlyNewWinners.HasValue && onlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(onlyNewWinners.HasValue && !onlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))  
                .Where(x => mainIds.Contains(x.UniversityId))
                .Where(x=>x.IsWinner == true)
                .Where(x => x.GrantId == grantid)
                .GroupBy(x => x.UniversityId)
                .ToDictionaryAsync(x => x.Key, y => y.Select(z=>z.StudentId).ToList());

            var addWinnerDict = await GetAll()
                .Include(x => x.Student)
                .WhereIf(onlyNewWinners.HasValue && onlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(onlyNewWinners.HasValue && !onlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))  
                .Where(x => addIds.Contains(x.UniversityId))
                .Where(x => x.IsAdditionalWinner == true)
                .Where(x => x.GrantId == grantid)
                .GroupBy(x => x.UniversityId)
                .ToDictionaryAsync(x => x.Key, y => y.Select(z=>z.StudentId).ToList());

            var nowticks = DateTime.Now.Ticks;
            
            var folderPath = Path.Combine(StoragePath, nowticks.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filename = "Победители.txt";

            var filepath = Path.Combine(folderPath, filename);

            TextWriter tw = new StreamWriter(filepath);

            

            foreach (var univer in mainUniverList)
            {
                tw.WriteLine(univer.Name);

                var studentsIds = winnerDict.ContainsKey(univer.UniversityId)
                    ? winnerDict[univer.UniversityId]
                    : null;

                var students = new List<Student>();

                if (studentsIds != null)
                {
                    var i = 1;
                    foreach (var studentId in studentsIds)
                    {
                        var student = studentDict[studentId];
                        students.Add(student);
                    }

                    students = students.OrderBy(x => x.Name).ThenBy(x => x.LastName).ThenBy(x => x.Patronymic).ToList();

                    foreach (var student in students)
                    {
                        tw.WriteLine(String.Format("{0}. {1} {2} {3}", i, student.Name, student.LastName, student.Patronymic));
                        i++;
                    }

                   
                }

                tw.WriteLine("");
            }

            tw.Close();

            filename = "Дополнительные победители.txt";

            filepath = Path.Combine(folderPath, filename);

            tw = new StreamWriter(filepath);

            foreach (var univer in additionalUniverList)
            {
                tw.WriteLine(univer.Name);

                var studentsIds = addWinnerDict.ContainsKey(univer.UniversityId)
                    ? addWinnerDict[univer.UniversityId]
                    : null;

                var students = new List<Student>();

                if (studentsIds != null)
                {
                    var i = 1;
                    foreach (var studentId in studentsIds)
                    {
                        var student = studentDict[studentId];
                        students.Add(student);
                    }

                    students = students.OrderBy(x => x.Name).ThenBy(x => x.LastName).ThenBy(x => x.Patronymic).ToList();

                    foreach (var student in students)
                    {
                        tw.WriteLine(String.Format("{0}. {1} {2} {3}", i, student.Name, student.LastName, student.Patronymic));
                        i++;
                    }
                }

                tw.WriteLine("");
            }

            tw.Close();

            var zipfileName = "Cписок победителей_" + DateTime.Now.ToString("dd.MM.yyyy") + "_" +
                            DateTime.Now.ToString("hh.mm.ss") + ".zip";

            using (var zip = new ZipFile())
            {
                //zip.TempFileFolder = folderPath;
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(folderPath + "\\");
                zip.Save(Path.Combine(StoragePath, zipfileName));

                return DataResult.Ok(new { path = "../reports/" + zipfileName, fileName = zipfileName });
            }

            return DataResult.Ok();
        }

        public async Task<DataResult> GetStudentsReport(long grantid, long univerId, bool additional = false)
        {
            var report = (GrantStudentsReport)Container.GetInstance<IReport>("GrantStudentsReport");
            var generator = Container.GetInstance<IReportGenerator>();

            report.UniversityId = univerId;
            report.GrantId = grantid;
            report.AdditionalStudents = additional;

            var filename = DateTime.Now.Ticks + report.GetFileName();
            string fileGuid;

            using (var ms = new MemoryStream())
            {
                await generator.GenerateAsync(ms, report);
                ms.Seek(0, SeekOrigin.Begin);
                fileGuid = await _fileManager.Upload(ms, filename);
            }

            var fileInfo = await _fileManager.Get(fileGuid);

            return DataResult.Ok(new { path = fileInfo.VirtualPath, fileName = report.GetFileName() });
        }

        public async Task<DataResult> GetAllAdditionalReport(long grantid, bool? onlyNewWinners)
        {
            var report = (GrantStudentsReport)Container.GetInstance<IReport>("GrantStudentsReport");
            var generator = Container.GetInstance<IReportGenerator>();

            report.UniversityId = 0;
            report.GrantId = grantid;
            report.AllAdditional = true;
            report.OnlyNewWinners = onlyNewWinners;

            var filename = DateTime.Now.Ticks + report.GetFileName();
            string fileGuid;

            using (var ms = new MemoryStream())
            {
                await generator.GenerateDocAsync(ms, report);
                ms.Seek(0, SeekOrigin.Begin);
                fileGuid = await _fileManager.Upload(ms, filename);
            }

            var fileInfo = await _fileManager.Get(fileGuid);

            return DataResult.Ok(new { path = fileInfo.VirtualPath, fileName = report.GetFileName() });
        }


        public async Task<DataResult> GetFullGrantOtherReport(long grantid,bool? onlyNewWinners,  long univerId = 0 , long activ =0)
        {
            var townList =

                   // await _grantQuotaRepo.GetAll()
                   //.Include(x => x.University)
                   //.Where(x => x.GrantId == grantid)
                   //.WhereIf(univerId != 0, x => x.UniversityId == univerId)
               (await _grantUniversityRepo.GetAll()
                .Include(x => x.University)
               .Include(x => x.Student)
               .Include(x => x.Student.University)
               .Where(x => x.University.Town != null)
                // .WhereIf(!additional, x => x.WinnerReport != null && x.WinnerReport.Length > 1)
                //.WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
               .Select(x => new
               {
                   x.University.Town
               }).ToListAsync())
               .Distinct()
               .OrderBy(x => x.Town).ToList();

            //var univerList = await _grantQuotaRepo.GetAll()
            //    .Include(x => x.University)jjk
            //    .Where(x => x.GrantId == grantid)
            //    .WhereIf(univerId != 0, x => x.UniversityId == univerId)
            //   // .WhereIf(!additional, x => x.WinnerReport != null && x.WinnerReport.Length > 1)
            //    //.WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
            //    .Select(x => new
            //    {
            //        x.UniversityId,
            //        x.University.Name
            //    }).OrderBy(x => x.Name).ToListAsync();


            var univerList =
               (await _grantUniversityRepo.GetAll()
               .Include(x => x.Student)
               .Include(x => x.Student.University)
              // .Include(x => x.University)
              .Where(x => x.GrantId == grantid && x.IsAdditionalWinner)
              //.Where(x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
              .Select(x => new
              {
                  x.UniversityId,
                  x.University.Name
              })
              .OrderBy(x => x.Name).ToListAsync())
              .Distinct(x => x.UniversityId)
              .ToList();


            var oldWinners = await
             GetAll()
            .Include(x => x.Student)
            .Where(x => x.GrantId != grantid)
            .WhereIf(activ ==0, x => x.IsWinner)
            .WhereIf(activ!=0,x=> x.IsAdditionalWinner)
            .Select(x => x.StudentId).ToArrayAsync();

            var otherStudentDict = GetAll()
                .Include(x => x.Student)
                .Include(x=>x.University)
                .Where(x => x.GrantId == grantid)
                .WhereIf(onlyNewWinners.HasValue && onlyNewWinners.Value, x => !oldWinners.Contains(x.StudentId))
                .WhereIf(onlyNewWinners.HasValue && !onlyNewWinners.Value, x => oldWinners.Contains(x.StudentId))
                .WhereIf(activ == 0, x => x.IsWinner)
               .WhereIf(activ != 0, x => x.IsAdditionalWinner)
               .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.University.Town)
               .ToDictionary(x => x.Key, y => y.Select(z => z.StudentId).ToList());


            var studentDict = await _studentrepo.GetAll()
                .Include(x => x.PersonalInfo)
               .Where(x => x.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.Id).ToDictionaryAsync(z => z.Key, y => y.First());

            var persIds = studentDict.Values.Select(x => x.PersonalInfoId).ToArray();


            var otherStudentsDocDict = (await personalInfoService.GetAll()
                .Include(x => x.PassportPage1)
                .Include(x => x.PassportPage4)
                .Include(x => x.PassportPage5)
                .Include(x => x.PassportPage7)
                .Include(x => x.PassportPage6)
                .Include(x => x.PassportPage10)
                .Where(x => persIds.Contains(x.Id))
                .ToListAsync())
                .GroupBy(x => x.Id)
                .ToDictionary(z => z.Key, y => y.First());


            var nowticks = DateTime.Now.Ticks;
            var folderPath = Path.Combine(StoragePath, nowticks.ToString());

            var dbfPath = Path.Combine(folderPath, "dbf");
           // var dbfRfPath = Path.Combine(dbfPath, "РФ");
            //var dbfOtherPath = Path.Combine(dbfPath, "Иное");
            var printPath = Path.Combine(folderPath, "Для печати");
            //var printRfPath = Path.Combine(printPath, "РФ");
            //var printOtherPath = Path.Combine(printPath, "Иное");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!Directory.Exists(dbfPath))
            {
                Directory.CreateDirectory(dbfPath);
            }

            if (!Directory.Exists(printPath))
            {
                Directory.CreateDirectory(printPath);
            }

            //if (!Directory.Exists(dbfRfPath))
            //{
            //    Directory.CreateDirectory(dbfRfPath);
            //}

            //if (!Directory.Exists(printRfPath))
            //{
            //    Directory.CreateDirectory(printRfPath);
            //}

            //if (!Directory.Exists(printOtherPath))
            //{
            //    Directory.CreateDirectory(printOtherPath);
            //}

            //if (!Directory.Exists(dbfOtherPath))
            //{
            //    Directory.CreateDirectory(dbfOtherPath);
            //}  

            Parallel.ForEach(townList, townElem =>
            {
                // генерация pdf
                var report = (GrantWinnerReport)Container.GetInstance<IReport>("GrantWinnerReport");
                var generator = Container.GetInstance<IReportGenerator>();

                report.AdditionalStudents = true;
                report.GrantId = grantid;
                report.AllWinners = false;
                report.Town = townElem.Town;
                report.OnlyNewWinners = onlyNewWinners;

                var filename = townElem.Town + ".doc";

                report.Citizenship = Citizenship.Other;

                using (var ms = new MemoryStream())
                {
                    generator.GenerateDoc(ms, report);
                    ms.Seek(0, SeekOrigin.Begin);

                    if (report.Count > 0)
                    {
                        var path = Path.Combine(printPath, filename);

                        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            ms.CopyTo(fileStream);
                        }
                    }
                }

                var dbfOtherUniverPath = Path.Combine(dbfPath, townElem.Town);

                var otherStudentsIds = otherStudentDict.ContainsKey(townElem.Town)
                                    ? otherStudentDict[townElem.Town]
                                    : null;

                if (otherStudentsIds != null)
                {
                    if (!Directory.Exists(dbfOtherUniverPath))
                    {
                        Directory.CreateDirectory(dbfOtherUniverPath);
                    }

                    foreach (var otherStudentId in otherStudentsIds)
                    {

                        var otherStudent = studentDict[otherStudentId];

                        var dbfOtherStudentPath = Path.Combine(dbfOtherUniverPath,
                             String.Format("{0} {1} {2}", otherStudent.Name, otherStudent.LastName, otherStudent.Patronymic));


                        if (!Directory.Exists(dbfOtherStudentPath))
                        {
                            Directory.CreateDirectory(dbfOtherStudentPath);
                        }

                        otherStudent.PersonalInfo = otherStudentsDocDict.ContainsKey(otherStudent.PersonalInfoId)
                            ? otherStudentsDocDict[otherStudent.PersonalInfoId]
                            : null;

                        if (otherStudent.PersonalInfo != null)
                        {

                            if (otherStudent.PersonalInfo.PassportPage1 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage1;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath,
                                        "Скан иностранного паспорта" + file.Extension);
                                    File.Copy(filePath, dest, true);
                                }
                            }

                            if (otherStudent.PersonalInfo.PassportPage4 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage4;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath, "Скан регистрации" + file.Extension);
                                    File.Copy(filePath, dest,true);
                                }
                            }

                            if (otherStudent.PersonalInfo.PassportPage5 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage5;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath,
                                        "Скан миграционной карты" + file.Extension);
                                    File.Copy(filePath, dest, true);
                                }
                            }

                            if (otherStudent.PersonalInfo.PassportPage7 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage7;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath,
                                        "Скан русской страницы в иностранном паспорте" + file.Extension);
                                    File.Copy(filePath, dest, true);
                                }
                            }

                            if (otherStudent.PersonalInfo.PassportPage6 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage6;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath,
                                        "Скан вида на жительство" + file.Extension);
                                    File.Copy(filePath, dest, true);
                                }
                            }

                            if (otherStudent.PersonalInfo.PassportPage10 != null)
                            {
                                var file = otherStudent.PersonalInfo.PassportPage10;
                                var filePath = file.Path;

                                if (File.Exists(filePath))
                                {
                                    var dest = Path.Combine(dbfOtherStudentPath,
                                        "Скан оборотной стороны регистрации" + file.Extension);
                                    File.Copy(filePath, dest, true);
                                }
                            }
                        }
                    }
                }

            });

            var filesCount = Directory.GetDirectories(dbfPath).Count() + Directory.GetFiles(dbfPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(dbfPath);
            }

            filesCount = Directory.GetDirectories(printPath).Count() + Directory.GetFiles(printPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(printPath);
            }

            //filesCount = Directory.GetDirectories(dbfOtherPath).Count() + Directory.GetFiles(dbfOtherPath).Count();
            //if (filesCount == 0)
            //{
            //    Directory.Delete(dbfOtherPath);
            //}


            //filesCount = Directory.GetDirectories(printOtherPath).Count() + Directory.GetFiles(printOtherPath).Count();
            //if (filesCount == 0)
            //{
            //    Directory.Delete(printOtherPath);
            //}

            var zipfileName =  "Выгрузка_иностранцев_" + DateTime.Now.ToString("dd.MM.yyyy") + "_" +
                              DateTime.Now.ToString("hh.mm.ss") + ".zip";

            using (var zip = new ZipFile())
            {
                //zip.TempFileFolder = folderPath;
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(folderPath + "\\");
                zip.Save(Path.Combine(StoragePath, zipfileName));

                return DataResult.Ok(new { path = "../reports/" + zipfileName, fileName = zipfileName });
            }

        }

        
        private string SafePath(string path)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) { path = path.Replace(c, ' '); }

            return path;
        }

        public async Task<DataResult> GetMainFullGrantReport(long grantid,  bool? onlyNewWinners,  bool additional = false, long univerId = 0)
        {

            var univerList = await _grantQuotaRepo.GetAll()
                .Include(x => x.University)
                .Where(x => x.GrantId == grantid)
                .WhereIf(univerId != 0, x => x.UniversityId == univerId)
                .WhereIf(!additional, x => x.WinnerReport != null && x.WinnerReport.Length > 1)
                //.WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
                .Select(x => new
                {
                    x.UniversityId,
                    x.University.Name,
                    x.University.Town
                }).ToListAsync();
                

            var otherStudentDict = GetAll().Include(x => x.Student)
               .WhereIf(additional, x => x.IsAdditionalWinner)
               .WhereIf(!additional, x => x.IsWinner)
               .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.UniversityId)
               .ToDictionary(x => x.Key, y => y.Select(z => z.StudentId).ToList());


            var studentDict = await _studentrepo.GetAll()
                .Include(x => x.PersonalInfo)
               .Where(x => x.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.Id).ToDictionaryAsync(z => z.Key, y => y.First());

            var persIds = studentDict.Values.Select(x => x.PersonalInfoId).ToArray();


            var otherStudentsDocDict = (await personalInfoService.GetAll()
                .Include(x => x.PassportPage1)
                .Include(x => x.PassportPage4)
                .Include(x => x.PassportPage5)
                .Include(x => x.PassportPage7)
                .Include(x => x.PassportPage6)
                .Where(x => persIds.Contains(x.Id))
                .ToListAsync())
                .GroupBy(x => x.Id)
                .ToDictionary(z => z.Key, y => y.First());


            var nowticks = DateTime.Now.Ticks;
            var folderPath = Path.Combine(StoragePath, nowticks.ToString());

            var dbfPath = Path.Combine(folderPath, "dbf");
            //var dbfRfPath = Path.Combine(dbfPath, "РФ");
            // var dbfOtherPath = Path.Combine(dbfPath, "Иное");
            var printPath = Path.Combine(folderPath, "Для печати");
            // var printRfPath = Path.Combine(printPath, "РФ");
            //   var printOtherPath = Path.Combine(printPath, "Иное");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!Directory.Exists(dbfPath))
            {
                Directory.CreateDirectory(dbfPath);
            }

            if (!Directory.Exists(printPath))
            {
                Directory.CreateDirectory(printPath);
            }

            //if (!Directory.Exists(dbfRfPath))
            //{
            //    Directory.CreateDirectory(dbfRfPath);
            //}

            //if (!Directory.Exists(printRfPath))
            //{
            //    Directory.CreateDirectory(printRfPath);
            //}



          //  Parallel.ForEach(univerList, univer =>
            //    {

              foreach(var univer in univerList)
              { 
                    var townDbfPath = Path.Combine(dbfPath, SafePath(univer.Town));
                    var townPrintPath = Path.Combine(printPath, SafePath(univer.Town));

                    if (!Directory.Exists(townDbfPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(townDbfPath);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                       
                    }

                    if (!Directory.Exists(townPrintPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(townPrintPath);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    var univerName = SafePath(univer.Name.Replace("«", "").Replace("»", ""));


                    // генерация pdf
                    var report = (GrantWinnerReport) Container.GetInstance<IReport>("GrantWinnerReport");
                    var generator = Container.GetInstance<IReportGenerator>();

                    report.GrantId = grantid;
                    report.AdditionalStudents = additional;
                    report.ByUniversity = true;
                    report.Town = univerName;
                    report.UniversityId = univer.UniversityId;
                    report.OnlyNewWinners = onlyNewWinners;

                    var filename = univerName + ".doc";

                    report.Citizenship = Citizenship.Rf;

                    using (var ms = new MemoryStream())
                    {
                        generator.GenerateDoc(ms, report);
                        ms.Seek(0, SeekOrigin.Begin);

                        if (report.Count > 0)
                        {
                            var path = Path.Combine(townPrintPath, filename);

                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                            {
                                ms.CopyTo(fileStream);
                            }
                        }
                    }

                    var dbf = _dbfReportGenerator.GetDbfReport(grantid, null, univer.UniversityId, additional, true, onlyNewWinners);

                    if (dbf.Length != 0)
                    {
                        dbf.Seek(0, SeekOrigin.Begin);

                        filename = univerName + ".dbf";

                        var dbfpath = Path.Combine(townDbfPath, filename);

                        using (var fileStream = new FileStream(dbfpath, FileMode.Create, FileAccess.Write))
                        {
                            dbf.CopyTo(fileStream);
                        }
                    }

                }
              
              //);




            foreach (var univer in univerList)
            {

                var townDbfPath = Path.Combine(dbfPath, SafePath(univer.Town));
                var townPrintPath = Path.Combine(printPath, SafePath(univer.Town));

                if (Directory.Exists(townDbfPath))
                {
                    var filesCount2 = Directory.GetFiles(townDbfPath).Count();

                    if (filesCount2 == 0)
                    {
                        Directory.Delete(townDbfPath);
                    }
                }

                if (Directory.Exists(townPrintPath))
                {
                    var filesCount2 = Directory.GetFiles(townPrintPath).Count();
                    if (filesCount2 == 0)
                    {
                        Directory.Delete(townPrintPath);
                    }
                }
            }

            var filesCount = Directory.GetDirectories(dbfPath).Count() + Directory.GetFiles(dbfPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(dbfPath);
            }

            filesCount = Directory.GetDirectories(printPath).Count() + Directory.GetFiles(printPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(printPath);
            }

            var zipfileName = "Выгрузка_основных_победителей_" + DateTime.Now.ToString("dd.MM.yyyy") + "_" +
                              DateTime.Now.ToString("hh.mm.ss") + ".zip";

            using (var zip = new ZipFile())
            {
                //zip.TempFileFolder = folderPath;
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(folderPath + "\\");
                zip.Save(Path.Combine(StoragePath, zipfileName));

                return DataResult.Ok(new { path = "../reports/" + zipfileName, fileName = zipfileName });
            }

            return DataResult.Ok();
        }


        public async Task<DataResult> GetFullGrantReport(long grantid,  bool? onlyNewWinners, bool additional = false, long univerId = 0)
        {

            var townList = await _grantQuotaRepo.GetAll()
                .Include(x => x.University)
                .Where(x => x.GrantId == grantid)
                .WhereIf(univerId != 0, x => x.UniversityId == univerId)
                  .Where(x => x.University.Town != null)
               // .WhereIf(!additional, x => x.WinnerReport != null && x.WinnerReport.Length > 1)
                //.WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
                .Select(x => new
                {
                    x.University.Town
                })
                .Distinct()
                .OrderBy(x => x.Town).ToListAsync();

            var univerList = await _grantQuotaRepo.GetAll()
                .Include(x => x.University)
                .Where(x => x.GrantId == grantid)
                .WhereIf(univerId != 0, x=>x.UniversityId == univerId)
                .WhereIf(!additional, x => x.WinnerReport != null && x.WinnerReport.Length > 1)
                //.WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
                .Select(x => new
                {
                    x.UniversityId,
                    x.University.Name
                }).OrderBy(x => x.Name).ToListAsync();

            var otherStudentDict = GetAll().Include(x => x.Student)
               .WhereIf(additional, x => x.IsAdditionalWinner)
               .WhereIf(!additional, x => x.IsWinner)
               .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.UniversityId)
               .ToDictionary(x => x.Key, y => y.Select(z => z.StudentId).ToList());


            var studentDict = await _studentrepo.GetAll()
                .Include(x => x.PersonalInfo)
               .Where(x=>x.PersonalInfo.Citizenship == Citizenship.Other)
               .GroupBy(x => x.Id).ToDictionaryAsync(z => z.Key, y => y.First());

            var persIds = studentDict.Values.Select(x => x.PersonalInfoId).ToArray();


            var otherStudentsDocDict = (await personalInfoService.GetAll()
                .Include(x => x.PassportPage1)
                .Include(x => x.PassportPage4)
                .Include(x => x.PassportPage5)
                .Include(x => x.PassportPage7)
                .Include(x => x.PassportPage6)
                .Where(x=>persIds.Contains(x.Id))
                .ToListAsync())
                .GroupBy(x => x.Id)
                .ToDictionary(z => z.Key, y => y.First());


            var nowticks = DateTime.Now.Ticks;
            var folderPath = Path.Combine(StoragePath, nowticks.ToString());

            var dbfPath = Path.Combine(folderPath, "dbf");
            //var dbfRfPath = Path.Combine(dbfPath, "РФ");
            // var dbfOtherPath = Path.Combine(dbfPath, "Иное");
            var printPath = Path.Combine(folderPath, "Для печати");
           // var printRfPath = Path.Combine(printPath, "РФ");
         //   var printOtherPath = Path.Combine(printPath, "Иное");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!Directory.Exists(dbfPath))
            {
                Directory.CreateDirectory(dbfPath);
            }

            if (!Directory.Exists(printPath))
            {
                Directory.CreateDirectory(printPath);
            }

            //if (!Directory.Exists(dbfRfPath))
            //{
            //    Directory.CreateDirectory(dbfRfPath);
            //}

            //if (!Directory.Exists(printRfPath))
            //{
            //    Directory.CreateDirectory(printRfPath);
            //}

            Parallel.ForEach(townList, townElem =>
            {
                // генерация pdf
                var report = (GrantWinnerReport)Container.GetInstance<IReport>("GrantWinnerReport");
                var generator = Container.GetInstance<IReportGenerator>();

                report.GrantId = grantid;
                report.AdditionalStudents = additional;
                report.Town = townElem.Town;
                report.OnlyNewWinners = onlyNewWinners;

                var filename = townElem.Town + ".doc";

                report.Citizenship = Citizenship.Rf;

                using (var ms = new MemoryStream())
                {
                    generator.GenerateDoc(ms, report);
                    ms.Seek(0, SeekOrigin.Begin);

                    if (report.Count > 0)
                    {
                        var path = Path.Combine(printPath, filename);

                        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            ms.CopyTo(fileStream);
                        }
                    }
                }

                var dbf = _dbfReportGenerator.GetDbfReport(grantid, townElem.Town, 0, additional, false, onlyNewWinners);

                if (dbf.Length != 0)
                {
                    dbf.Seek(0, SeekOrigin.Begin);

                    filename = townElem.Town + ".dbf";

                    var dbfpath = Path.Combine(dbfPath, filename);

                    using (var fileStream = new FileStream(dbfpath, FileMode.Create, FileAccess.Write))
                    {
                        dbf.CopyTo(fileStream);
                    }
                }

            });

            var filesCount = Directory.GetDirectories(dbfPath).Count() + Directory.GetFiles(dbfPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(dbfPath);
            }

            filesCount = Directory.GetDirectories(printPath).Count() + Directory.GetFiles(printPath).Count();
            if (filesCount == 0)
            {
                Directory.Delete(printPath);
            }

            var zipfileName = "Выгрузка_дополнительных_победителей_" + DateTime.Now.ToString("dd.MM.yyyy") + "_" +
                              DateTime.Now.ToString("hh.mm.ss") + ".zip";

            using (var zip = new ZipFile())
            {
                //zip.TempFileFolder = folderPath;
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(folderPath + "\\");
                zip.Save(Path.Combine(StoragePath, zipfileName));

                return DataResult.Ok(new { path = "../reports/" + zipfileName, fileName = zipfileName });
            }

            return DataResult.Ok();
        }




        //public async Task<DataResult> GetOldFullGrantReport(long grantid, bool additional = false)
        //{

        //    var univerList = await _grantQuotaRepo.GetAll()
        //        .Include(x => x.University)
        //        .Where(x => x.GrantId == grantid)
        //        .WhereIf(!additional, x=> x.WinnerReport != null && x.WinnerReport.Length > 1)
        //        .WhereIf(additional, x => x.AdditionalWinnerReport != null && x.AdditionalWinnerReport.Length > 1)
        //        .Select(x => new
        //        {
        //            x.UniversityId,
        //            x.University.Name
        //        }).OrderBy(x => x.Name).ToListAsync();


        //    var otherStudentDict = GetAll().Include(x => x.Student.PersonalInfo)
        //        .Include(x=>x.Student.PersonalInfo.PassportPage1)
        //        .Include(x => x.Student.PersonalInfo.PassportPage4)
        //        .Include(x => x.Student.PersonalInfo.PassportPage5)
        //        .Include(x => x.Student.PersonalInfo.PassportPage7)
        //        .Include(x => x.Student.PersonalInfo.PassportPage6)
        //        .WhereIf(additional, x => x.IsAdditionalWinner)
        //        .WhereIf(!additional, x => x.IsWinner)
        //        .Where(x => x.Student.PersonalInfo.Citizenship == Citizenship.Other)
        //        .GroupBy(x => x.UniversityId)
        //        .ToDictionary(x => x.Key, y => y.Select(z => z.Student).ToList());


        //    var nowticks = DateTime.Now.Ticks;
        //    var folderPath = Path.Combine(StoragePath, nowticks.ToString());

        //    var dbfPath = Path.Combine(folderPath, "dbf");
        //    var dbfRfPath = Path.Combine(dbfPath, "РФ");
        //    var dbfOtherPath = Path.Combine(dbfPath, "Иное");
        //    var printPath = Path.Combine(folderPath, "Для печати");
        //    var printRfPath = Path.Combine(printPath, "РФ");
        //    var printOtherPath = Path.Combine(printPath, "Иное");

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    if (!Directory.Exists(dbfPath))
        //    {
        //        Directory.CreateDirectory(dbfPath);
        //    }

        //    if (!Directory.Exists(printPath))
        //    {
        //        Directory.CreateDirectory(printPath);
        //    }

        //    if (!Directory.Exists(dbfRfPath))
        //    {
        //        Directory.CreateDirectory(dbfRfPath);
        //    }

        //    if (!Directory.Exists(printRfPath))
        //    {
        //        Directory.CreateDirectory(printRfPath);
        //    }

        //    if (!Directory.Exists(printOtherPath))
        //    {
        //        Directory.CreateDirectory(printOtherPath);
        //    }


            

        //    foreach (var univer in univerList)
        //    {
        //        // генерация pdf
        //        var report = (GrantWinnerReport)Container.GetInstance<IReport>("GrantWinnerReport");
        //        var generator = Container.GetInstance<IReportGenerator>();

        //        report.UniversityId = univer.UniversityId;
        //        report.GrantId = grantid;
        //        report.AdditionalStudents = additional;
        //        report.UniversityName = univer.Name;

        //        var filename = report.GetFileName();

        //        report.Citizenship = Citizenship.Rf;

        //        using (var ms = new MemoryStream())
        //        {
        //            await generator.GenerateAsync(ms, report);
        //            ms.Seek(0, SeekOrigin.Begin);

        //            var path = Path.Combine(printRfPath, filename);

        //            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        //            {
        //                await ms.CopyToAsync(fileStream);
        //            }
        //        }

        //        report.Citizenship = Citizenship.Other;

        //        using (var ms = new MemoryStream())
        //        {
        //            await generator.GenerateAsync(ms, report);
        //            ms.Seek(0, SeekOrigin.Begin);

        //            var path = Path.Combine(printOtherPath, filename);

        //            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        //            {
        //                await ms.CopyToAsync(fileStream);
        //            }
        //        }

        //        var dbf = await _dbfReportGenerator.GetDbfReportAsync(grantid, univer.UniversityId, additional);

        //        if (dbf.Length != 0)
        //        {
        //            dbf.Seek(0, SeekOrigin.Begin);

        //            filename = univer.Name + ".dbf";

        //            var dbfpath = Path.Combine(dbfRfPath, filename);

        //            using (var fileStream = new FileStream(dbfpath, FileMode.Create, FileAccess.Write))
        //            {
        //                await dbf.CopyToAsync(fileStream);
        //            }
        //        }

        //        var otherStudents = otherStudentDict.ContainsKey(univer.UniversityId)
        //            ? otherStudentDict[univer.UniversityId]
        //            : null;

        //        if (otherStudents != null)
        //        {
        //            foreach (var otherStudent in otherStudents)
        //            {
        //                var dbfOtherStudentPath = Path.Combine(dbfOtherPath, 
        //                    String.Format("{0} {1} {2} {3}", otherStudent.Id, otherStudent.Name, otherStudent.LastName, otherStudent.Patronymic));


        //                if (!Directory.Exists(dbfOtherStudentPath))
        //                {
        //                    Directory.CreateDirectory(dbfOtherStudentPath);
        //                }

        //                if (otherStudent.PersonalInfo.PassportPage1 != null)
        //                {
        //                    var file = otherStudent.PersonalInfo.PassportPage1;
        //                    var filePath = file.Path;

        //                    if (File.Exists(filePath))
        //                    {
        //                        var dest = Path.Combine(dbfOtherStudentPath, "Скан иностранного паспорта" + file.Extension);
        //                        File.Copy(filePath, dest);
        //                    }
        //                }

        //                if (otherStudent.PersonalInfo.PassportPage4 != null)
        //                {
        //                    var file = otherStudent.PersonalInfo.PassportPage4;
        //                    var filePath = file.Path;

        //                    if (File.Exists(filePath))
        //                    {
        //                        var dest = Path.Combine(dbfOtherStudentPath, "Скан регистрации" + file.Extension);
        //                        File.Copy(filePath, dest);
        //                    }
        //                }

        //                if (otherStudent.PersonalInfo.PassportPage5 != null)
        //                {
        //                    var file = otherStudent.PersonalInfo.PassportPage5;
        //                    var filePath = file.Path;

        //                    if (File.Exists(filePath))
        //                    {
        //                        var dest = Path.Combine(dbfOtherStudentPath, "Скан миграционной карты" + file.Extension);
        //                        File.Copy(filePath, dest);
        //                    }
        //                }

        //                if (otherStudent.PersonalInfo.PassportPage7 != null)
        //                {
        //                    var file = otherStudent.PersonalInfo.PassportPage7;
        //                    var filePath = file.Path;

        //                    if (File.Exists(filePath))
        //                    {
        //                        var dest = Path.Combine(dbfOtherStudentPath, "Скан русской страницы в иностранном паспорте" + file.Extension);
        //                        File.Copy(filePath, dest);
        //                    }
        //                }

        //                if (otherStudent.PersonalInfo.PassportPage6 != null)
        //                {
        //                    var file = otherStudent.PersonalInfo.PassportPage6;
        //                    var filePath = file.Path;

        //                    if (File.Exists(filePath))
        //                    {
        //                        var dest = Path.Combine(dbfOtherStudentPath, "Скан вида на жительство" + file.Extension);
        //                        File.Copy(filePath, dest);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var zipfileName = DateTime.Now.ToString("dd.MM.yyyy") + "_" +
        //                      DateTime.Now.ToString("hh.mm.ss") + ".zip";

        //    using (var zip = new ZipFile())
        //    {
        //        //zip.TempFileFolder = folderPath;
        //        zip.UseUnicodeAsNecessary = true;  // utf-8
        //        zip.AddDirectory(folderPath + "\\");
        //        zip.Save(Path.Combine(StoragePath, zipfileName));

        //        return DataResult.Ok(new { path = "../reports/" + zipfileName, fileName = zipfileName });
        //    }

            


        //    return DataResult.Ok();
        //}


        public async Task<DataResult> GetWinnersReport(long grantid, long univerId, bool additional = false)
        {
            var report = (GrantWinnerReport)Container.GetInstance<IReport>("GrantWinnerReport");
            var generator = Container.GetInstance<IReportGenerator>();

            var univerName = _universityService.Get(univerId).Result.Name;

            report.UniversityId = univerId;
            report.ByUniversity = true;
            report.GrantId = grantid;
            report.AdditionalStudents = additional;
            report.Town = univerName;

            var filename = DateTime.Now.Ticks + report.GetFileName();
            string fileGuid;

            using (var ms = new MemoryStream())
            {
                await generator.GenerateAsync(ms, report);
                ms.Seek(0, SeekOrigin.Begin);
                fileGuid = await _fileManager.Upload(ms, filename);
            }

            var fileInfo = await _fileManager.Get(fileGuid);

            return DataResult.Ok(new { path = fileInfo.VirtualPath, fileName = report.GetFileName() });
        }

        public async Task<DataResult> GetStudentsDbfReport(long grantid,  bool? onlyNewWinners, long univerId, bool additional = false)
        {
           //var result = await _dbfReportGenerator.GetDbfReportAsync(grantid, univerId, additional);

           //result.Seek(0, SeekOrigin.Begin);
           //var fileGuid = await _fileManager.Upload(result, "Выгрузка_в_банк.dbf");

           //var fileInfo = await _fileManager.Get(fileGuid);

          // return DataResult.Ok(new { path = fileInfo.VirtualPath, fileName = fileInfo.FileName });


            return await GetFullGrantReport(grantid, onlyNewWinners, additional, univerId);
        }

        public async Task<DataResult> GetUserGrants(long studentId)
        {
            
             var records = await GetAll().Include(x => x.Grant)
                .Where(x => x.StudentId == studentId)
                .ToListAsync();


            foreach (var rec in records)
            {
                if (!String.IsNullOrEmpty(rec.Grant.ImageFile))
                {
                    var image = await _fileManager.Get(rec.Grant.ImageFile);

                    if (image != null)
                    {
                        rec.Grant.ImageLink = image.VirtualPath;
                    }
                }

                if (String.IsNullOrEmpty(rec.Grant.ImageLink))
                {
                    rec.Grant.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
                }
            }

            var result = records.Select(x => new
            {
                x.Grant.Id,
                x.Grant.Name,
                x.Grant.Description,
                x.Grant.Status,
                x.Grant.ExpiresDate,
                x.IsWinner,
                x.IsAdditionalWinner,
                x.Grant.ImageLink,
            }).ToList();

            return DataResult.Ok(result);
        }

        public async Task<DataResult> GetStat(long grantId)
        {
            var result = new GrantStat();

            var grant = await _grantService.Get(grantId);

            result.Quota = grant.FullQuota.GetValueOrDefault(0);

            if (await _grantQuotaRepo.GetAll().Where(x => x.GrantId == grantId).AnyAsync())
            {
                result.UniversutyQuota = await _grantQuotaRepo.GetAll().Where(x => x.GrantId == grantId).SumAsync(x => x.Quota);
            }

            if (await GetAll().Where(x => x.GrantId == grantId).AnyAsync())
            {
                result.StudentCount = await GetAll().Where(x => x.GrantId == grantId).CountAsync();
            }

            if (await GetAll().Where(x => x.GrantId == grantId && x.IsWinner).AnyAsync())
            {
                result.WinnersCount =
                await GetAll().Where(x => x.GrantId == grantId && x.IsWinner).CountAsync();
            }

            if (await GetAll().Where(x => x.GrantId == grantId && x.IsAdditionalWinner).AnyAsync())
            {
               result.AdditionalWinnersCount =
               await GetAll().Where(x => x.GrantId == grantId && x.IsAdditionalWinner).CountAsync(); 
            }
           

            var notRfStudentCountQuery = GetAll()
                .Include(x=>x.Student)
                .Include(x => x.Student.PersonalInfo)
                .Where(x => x.GrantId == grantId && x.Student.PersonalInfo.Citizenship == Citizenship.Other);

            if (await notRfStudentCountQuery.AnyAsync())
            {
                result.NotRfStudentCount = await notRfStudentCountQuery.CountAsync();
            }
           
            var notRfWinnerCountQuery = GetAll()
                .Include(x => x.Student)
                .Include(x => x.Student.PersonalInfo)
                .Where(x => x.GrantId == grantId && (x.IsWinner) && x.Student.PersonalInfo.Citizenship == Citizenship.Other);

            if (await notRfWinnerCountQuery.AnyAsync())
            {
                result.NotRfWinnerCount = await notRfWinnerCountQuery.CountAsync();
            }

            var notRfAddWinnerCountQuery = GetAll()
                .Include(x => x.Student)
                .Include(x => x.Student.PersonalInfo)
                .Where(
                    x =>
                        x.GrantId == grantId && (x.IsAdditionalWinner) &&
                        x.Student.PersonalInfo.Citizenship == Citizenship.Other);

            if (await notRfWinnerCountQuery.AnyAsync())
            {
                result.NotRfAddWinnerCount =
                    await notRfAddWinnerCountQuery.CountAsync();
            }

            return DataResult.Ok(result);

        }


        public async Task<DataResult> SetPassportInvalidState(long id, string message, bool force = false)
        {
            var student = await _studentService.Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await _studentService.GetCurrent();


            var grantStudent = await GetAll().Include(x => x.Grant).Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.StudentId == id && x.GrantId > 27);

            if (grantStudent != null)
            {
                if (grantStudent.IsWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран победителем гранта. Замечание к его паспортным данным отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве победителя замечанием к паспортным данным",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора победителя", eventText, EventType.GrantWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
                else if (grantStudent.IsAdditionalWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран дополнительным победителем гранта. Замечание к его паспортным данным отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsAdditionalWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве дополнительного победителя  замечанием к паспортным данным",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора дополнительного победителя", eventText, EventType.GrantAdditionalWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
            }


                var rec = new ValidationHistory
                {
                    ValidationUserId = id,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Invalid,
                    Target = ValidationTarget.PersonalInfo,
                    ValidationMessage = message
                };

                await validationHistRepo.Create(rec);


            student.PassportState = ValidationState.Invalid;
            student.PassValidationComment = message;

            await _studentService.Update(student);
            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

            await
                 _notificator.Enqueue(student, null, NotificationType.InvalidData,
                    new Dictionary<string, object>
                    {
                        { "Password", message },
                        { "ProfileLink", serverAddress + "/#/user-profile/profile/" },
                        { "DomainName", serverAddress.Replace("http://", string.Empty) }
                    });

            return DataResult.Ok();
        }

        public async Task<DataResult> SetAchievementInvalidState(long id, string message, bool force = false)
        {
            var item = await achRepository.GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return DataResult.Failure("Достижение с таким id не найдено");
            }


            var student = await _studentService.Get(item.StudentId);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await _studentService.GetCurrent();


            var grantStudent = await GetAll().Include(x => x.Grant).Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.StudentId == id && x.GrantId > 27);

            if (grantStudent != null)
            {
                if (grantStudent.IsWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран победителем гранта. Замечание отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве победителя замечанием к достижению",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора победителя", eventText, EventType.GrantWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
                else if (grantStudent.IsAdditionalWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран дополнительным победителем гранта. Замечание к его паспортным данным отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsAdditionalWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве дополнительного победителя  замечанием к паспортным данным",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора дополнительного победителя", eventText, EventType.GrantAdditionalWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
            }


            var rec = new ValidationHistory
            {
                ValidationUserId = item.StudentId,
                ModeratorId = moderator.Id,
                State = ValidationState.Invalid,
                Target = ValidationTarget.Achievement,
                ValidationMessage = message
            };

            await validationHistRepo.Create(rec);


            item.ValidationState = ValidationState.Invalid;
            item.ValidationComment = message;

            await achRepository.Update(item);
            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

            await
                 _notificator.Enqueue(student, null, NotificationType.InvalidData,
                    new Dictionary<string, object>
                    {
                        { "Password", message },
                        { "ProfileLink", serverAddress + "/#/user-profile/profile/" },
                        { "DomainName", serverAddress.Replace("http://", string.Empty) }
                    });

            return DataResult.Ok();
        }

        public async Task<DataResult> SetIncomeInvalidState(long id, string message, bool force = false)
        {
            var student = await _studentService.Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await _studentService.GetCurrent();


            var grantStudent = await GetAll().Include(x => x.Grant).Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.StudentId == id && x.GrantId > 27);

            if (grantStudent != null)
            {
                if (grantStudent.IsWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран победителем гранта. Замечание отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве победителя замечанием к дыннм о доходе",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора победителя", eventText, EventType.GrantWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
                else if (grantStudent.IsAdditionalWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран дополнительным победителем гранта. Замечание к его данным о доходе отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsAdditionalWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве дополнительного победителя  замечанием к данным о доходе",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора дополнительного победителя", eventText, EventType.GrantAdditionalWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
            }


            var rec = new ValidationHistory
            {
                ValidationUserId = id,
                ModeratorId = moderator.Id,
                State = ValidationState.Invalid,
                Target = ValidationTarget.Income,
                ValidationMessage = message
            };

            await validationHistRepo.Create(rec);


            student.IncomeState = ValidationState.Invalid;
            student.IncomeValidationComment = message;

            await _studentService.Update(student);
            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

            await
                 _notificator.Enqueue(student, null, NotificationType.InvalidData,
                    new Dictionary<string, object>
                    {
                        { "Password", message },
                        { "ProfileLink", serverAddress + "/#/user-profile/profile/" },
                        { "DomainName", serverAddress.Replace("http://", string.Empty) }
                    });

            return DataResult.Ok();
        }

        public async Task<DataResult> SetBookInvalidState(long id, string message, bool force = false)
        {
            var student = await _studentService.Get(id);

            if (student == null)
            {
                return DataResult.Failure("Студент с таким id не найден");
            }

            var moderator = await _studentService.GetCurrent();

            var grantStudent = await GetAll().Include(x => x.Grant).Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.StudentId == id && x.GrantId > 27);

            if (grantStudent != null)
            {
                if (grantStudent.IsWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран победителем гранта. Замечание к его зачетной книжке отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве победителя замечанием к зачетной книжке",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора победителя", eventText, EventType.GrantWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
                else if (grantStudent.IsAdditionalWinner)
                {
                    if (!force)
                    {
                        return
                            DataResult.Failure(
                                "Студент выбран дополнительным победителем гранта. Замечание к его зачетной книжке отменит этот выбор");
                    }
                    else
                    {
                        grantStudent.IsAdditionalWinner = false;

                        var grant = grantStudent.Grant;
                        var stud = grantStudent.Student;

                        var eventText = String.Format("Модератор {0} {1} {2} отменил выбор {3} {4} {5} в качестве дополнительного победителя замечанием к зачетной книжке",
                            moderator.Name,
                            moderator.LastName,
                            moderator.Patronymic,
                            stud.Name,
                            stud.LastName,
                            stud.Patronymic);

                        await _grantEventService.CreateEvent(grant, "Отмена выбора дополнительного победителя", eventText, EventType.GrantAdditionalWinnerCanceled);
                        await Update(grantStudent);
                    }

                }
            }


                var rec = new ValidationHistory
                {
                    ValidationUserId = id,
                    ModeratorId = moderator.Id,
                    State = ValidationState.Invalid,
                    Target = ValidationTarget.RecordBook,
                    ValidationMessage = message
                };

                await validationHistRepo.Create(rec);
            

            student.StudentBookState = ValidationState.Invalid;
            student.BookValidationComment = message;

            await _studentService.Update(student);

            var serverAddress = ConfigurationManager.AppSettings["serverAddress"];

            await
                   _notificator.Enqueue(student, null, NotificationType.InvalidData,
                      new Dictionary<string, object>
                    {
                        { "Password", message },
                        { "ProfileLink", serverAddress + "/#/user-profile/profile/" },
                        { "DomainName", serverAddress.Replace("http://", string.Empty) }
                    });

            return DataResult.Ok();
        }
    }

}
