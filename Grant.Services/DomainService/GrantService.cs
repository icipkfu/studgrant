using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Web;
using Grant.Core.Context;
using Grant.Core.Notification;
using Grant.Core.UserIdentity;
using Grant.Utils.Extensions;
using Microsoft.AspNet.Identity.Owin;

namespace Grant.Services.DomainService
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;
    using Core.Enum;
    using DataAccess;

    internal class GrantService : BaseDomainService<Grant>, IGrantService
    {
        private readonly IFileManager _fileManager;

        private readonly IGrantEventService _eventService;

        private readonly IStudentService _studentService;

        private readonly IGrantAdminService _adminService;

        private readonly IRepository<GrantStudent> _studentGrantRepository;

        private readonly INotificationQueueProvider _notificationQueueProvider;

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                       (_userManager =
                           HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
        }

        public GrantService(IRepository<Grant> repository,
            IFileManager fileManager,
            IGrantEventService eventService,
            IStudentService studentService,
            IGrantAdminService adminService,
            INotificationQueueProvider notificationQueueProvider,
            IRepository<GrantStudent> studentGrantRepository)
            : base(repository)
        {
            _fileManager = fileManager;
            _eventService = eventService;
            _studentService = studentService;
            _notificationQueueProvider = notificationQueueProvider;
            _studentGrantRepository = studentGrantRepository;
            _adminService = adminService;
        }

        public override async Task<DataResult> Update(Grant entity, bool deferred = false)
        {
            var oldGrant = await Get(entity.Id);

            await
                _eventService.CreateEvent(entity, "Изменение карточки гранта", "Грант отредактирован",
                    EventType.GrantChanged, oldGrant);

            if (oldGrant == null ||
                String.Compare(oldGrant.Administrators, entity.Administrators, StringComparison.CurrentCulture) != 0)
            {
                await
                    _adminService.SetGrantAdmins(entity.Id,
                        entity.Administrators.Split(',').Select(x => x.ToLong()).ToArray());
            }


            Entry(oldGrant).State = EntityState.Detached;
            Entry(entity).State = EntityState.Modified;


            return await base.Update(entity, deferred: false);
        }

        public override async Task<DataResult> Create(Grant entity)
        {
            entity.Status = GrantStatus.Draft;
            entity.CreateDate = DateTime.Now;

            await base.Create(entity);

            await
                _eventService.CreateEvent(entity, "Создание гранта", "Грант создан в статусе черновик",
                    EventType.GrantCreated, oldGrant: null);

            await
                _adminService.SetGrantAdmins(entity.Id,
                    entity.Administrators.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.ToLong())
                        .ToArray());

            return DataResult.Ok(entity);
        }

        public override async Task<DataResult> Delete(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Удаляемый грант не найден");
            }

            grant.DeletedMark = true;
            grant.Status = GrantStatus.Deleted;

            await base.Update(grant);

            await _eventService.CreateEvent(grant, "Удаление гранта", "Грант удален", EventType.GrantDeleted);

            return DataResult.Ok();
        }


        public override async Task<Grant> Get(long id)
        {
            var grant = await base.Get(id);

            if (!String.IsNullOrEmpty(grant.ImageFile))
            {
                var image = await _fileManager.Get(grant.ImageFile);

                if (image != null)
                {
                    grant.ImageLink = image.VirtualPath;
                }
            }

            if (String.IsNullOrEmpty(grant.ImageLink))
            {
                grant.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
            }

            return grant;
        }

        public override async Task<IQueryable<Grant>> GetAllAsync()
        {
            var curStudent = await _studentService.GetCurrent();
            var roleInfo = await _studentService.GetCurrentRole();
            var curId = curStudent.Id;

            long[] specialUsers = {27396, 27397, 27398, 21338, 34, 42};
            var isSpecialUser = specialUsers.Contains(curStudent.Id);


            var isAdmin = (curStudent.Role == Role.Administrator) || roleInfo.UniversCurator.Any();

            var grantIds = await
                _studentGrantRepository.GetAll()
                    .Where(x => x.StudentId == curStudent.Id)
                    .Select(x => x.GrantId).ToArrayAsync();


            var result = (await base.GetAllAsync())
                .Where(x => isAdmin
                            ||
                            ((x.DeletedMark == false) &&
                             (x.Status == GrantStatus.Registration || grantIds.Contains(x.Id)))
                            || roleInfo.GrantsAdmin.Contains(x.Id))
                        
                .Where(x=>x.Id != 28 || isSpecialUser)
                .OrderByDescending(x => x.Id).ToList();

            foreach (var grant in result)
            {
                if (!String.IsNullOrEmpty(grant.ImageFile))
                {
                    var image = await _fileManager.Get(grant.ImageFile);

                    if (image != null)
                    {
                        grant.ImageLink = image.VirtualPath;
                    }
                }

                if (String.IsNullOrEmpty(grant.ImageLink))
                {
                    grant.ImageLink = "/assets/images/backgrounds/bg-5.jpg";
                }
            }

            return result.AsQueryable();
        }

        public async Task<DataResult> GetGrantList()
        {
            var result=  await GetAll().OrderBy(x => x.Name).Select(x => new
            {
                x.Id,
                x.Name
            }).ToListAsync();

            return DataResult.Ok(result);
        }

        public async Task<DataResult> ReturnToDraft(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.Draft;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Перевод в черновик", "Грант переведен в черновик",
                    EventType.GrantMovedToDraft);

            return result;
        }

        public async Task<DataResult> StartGrant(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.Registration;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Запуск гранта", "Грант переведен на этап регистрации участников",
                    EventType.GrantOpenRegistration);

            var studentsForNotify = await _studentService.GetAll()
                .Where(x => x.GrantStudents.All(g => g.GrantId != id))
                .ToListAsync();


            /*
            foreach (var student in studentsForNotify)
            {
                await
                    _notificationQueueProvider.Enqueue(student, grant, NotificationType.GrantCreated,
                        new Dictionary<string, object>
                        {
                            {"Name", grant.Name},
                            {"Description", grant.Description},
                            {"RegistrationEndDate", grant.ExpiresDate.ToShortDateString()},
                            {"Conditions", grant.Conditions},
                            {"Link", ApplicationContext.Current.GetUrl()}
                        });
            } */

            return result;
        }

        public async Task<DataResult> CloseRegistration(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.RegistrationFinished;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Регистрация закрыта", "Закрыта регистрация участников",
                    EventType.GrantCloseRegistration);

            return result;
        }


        public async Task<DataResult> СhangeCanAddReport(long id, bool option)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.CanAddReport = option;

            var result = await base.Update(grant);


            if (option)
            {
                await
                    _eventService.CreateEvent(grant, "Открыта возможность загружать отчет о победителях",
                        "Открыта возможность загружать отчет о победителях в " + grant.Name, EventType.GrantCanAddReport);
            }
            else
            {
                await
                  _eventService.CreateEvent(grant, "Закрыта возможность загружать отчет о победителях",
                      "Закрыта возможность загружать отчет о победителях в " + grant.Name, EventType.GrantAddReportNotAvailable);
            }
          

            return result;
        }

        public async Task<DataResult> OpenWinnersSelection(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.WinnersSelection;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Выбор победителей",
                    "Запущен этап выбора победителей", EventType.GrantOpenWinnersSelection);

            return result;
        }

        public async Task<DataResult> CloseWinnersSelection(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.WinnersSelectionClosed;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Закрыт выбор победителей",
                    "Выбор победителей конкурса завершен", EventType.GrantCloseWinnersSelection);

            return result;
        }

        public async Task<DataResult> OpenAdditionalWinnersSelection(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.AddtitionalSelection;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Выбор дополнительных победителей",
                    "Запущен этап выбора дополнительных победителей", EventType.GrantOpenAdditionalWinnerSelection);

            return result;
        }

        public async Task<DataResult> CloseAdditionalWinnersSelection(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.AddtitionalSelectionClosed;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Выбор дополнительных победителей завершен",
                    "Завершен этап выбора дополнительных победителей", EventType.GrantCloseAdditionalWinnerSelection);

            return result;
        }

        public async Task<DataResult> OpenDelivery(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.Delivery;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Выдача гранта победителям", "Запущен этап выдачи гранта победителям",
                    EventType.GrantOpenDelivery);

            return result;
        }

        public async Task<DataResult> CancelDelivery(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.DeliveryCancel;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Выдача гранта победителям отменена",
                    "Этап выдачи гранта победителям отменен", EventType.GrantCloseDelivery);

            return result;
        }

        public async Task<DataResult> OpenFinal(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.Final;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Завершение конкурса", "Запущен этап завершения конкурса",
                    EventType.GrantFinishOpen);

            return result;
        }

        public async Task<DataResult> CancelFinal(long id)
        {
            var grant = await Get(id);

            if (grant == null)
            {
                return DataResult.Failure("Не найден грант с заданным id");
            }

            grant.Status = GrantStatus.FinalCancel;

            var result = await base.Update(grant);

            await
                _eventService.CreateEvent(grant, "Завершение конкурса", "Этап завершения конкурса отменен",
                    EventType.GrantFinishCanceled);

            return result;
        }

        public async Task<IEnumerable<Student>> GetAdmins(long id)
        {
            var students = await _adminService.GetGrantAdmins(id);

            foreach (var student in students)
            {
                if (!String.IsNullOrEmpty(student.ImageFile))
                {
                    var image = await _fileManager.Get(student.ImageFile);

                    if (image != null)
                    {
                        student.ImageLink = image.VirtualPath;
                    }
                    else
                    {
                        student.ImageLink = student.Sex == Sex.Male ? @"/assets/images/grant/boy.png" : @"/assets/images/grant/girl.png";
                    }
                }
            }

            return students;
        }


        public async Task<object> GetGrantRegChart(long id)
        {
            var regData = (await _studentGrantRepository.GetAll()
                .Where(x => x.GrantId == id && !x.DeletedMark)
                .Select(x => new
                {
                   x.CreateDate,
                   x.Id
                })
                .ToListAsync())
                .Select(x=> new{
                    CreateDate = x.CreateDate.Date,
                    x.Id
                })
                .GroupBy(x=>x.CreateDate)
                .OrderBy(x=>x.Key)
                .ToDictionary(x=>x.Key, y=>y.Count());



            var repeatStudentIds = await _studentGrantRepository.GetAll()
                .Where(x => x.GrantId < id)
                .Select(x => x.StudentId).ToArrayAsync();

            var repeatRegData = (await _studentGrantRepository.GetAll()
                .Where(x => x.GrantId == id && !x.DeletedMark)
                .Select(x => new
                {

                    x.CreateDate,
                    x.StudentId,
                    x.Id
                })
                .ToListAsync())
                .Where(x => repeatStudentIds.Contains(x.StudentId))
                .Select(x => new
                {
                    CreateDate = x.CreateDate.Date,
                    x.Id
                })
                .GroupBy(x => x.CreateDate)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, y => y.Count());


            var repeatWinStudentIds = await _studentGrantRepository.GetAll()
               .Where(x => x.GrantId < id)
               .Where(x=>x.IsWinner || x.IsAdditionalWinner)
               .Select(x => x.StudentId).ToArrayAsync();


            var repeatWinRegData = (await _studentGrantRepository.GetAll()
               .Where(x => x.GrantId == id && !x.DeletedMark)
               .Select(x => new
               {

                   x.CreateDate,
                   x.StudentId,
                   x.Id
               })
               .ToListAsync())
               .Where(x => repeatWinStudentIds.Contains(x.StudentId))
               .Select(x => new
               {
                   CreateDate = x.CreateDate.Date,
                   x.Id
               })
               .GroupBy(x => x.CreateDate)
               .OrderBy(x => x.Key)
               .ToDictionary(x => x.Key, y => y.Count());



            List<int> rowReg = new List<int>();
            List<int> rowRepeatReg = new List<int>();
            List<int> rowRepeatWin = new List<int>();
            List<string> labels = new List<string>();

            int regCount = 0;
            int regRepeatCount = 0;
            int regRepeatWinCount = 0;

            var key = regData.Keys.Where(x => x.Year > 1).First();
            var maxkey = regData.Keys.Last();

            while(key <= maxkey)
            {
                if (regData.ContainsKey(key))
                {
                    regCount += regData[key];
                }
                

                if (key.Year == 1)
                {
                   

                }
                else
                {
                    var day = key.Day > 9 ? key.Day.ToString() : "0" + key.Day.ToString();
                    var mon = key.Month > 9 ? key.Month.ToString() : "0" + key.Month.ToString();
                    labels.Add(day + "." + mon);

                    //
                    rowReg.Add(regCount);

                    ///
                    if (repeatRegData.ContainsKey(key))
                    {
                        regRepeatCount += repeatRegData[key];
                    }
                    
                    rowRepeatReg.Add(regRepeatCount);


                    ////

                    if (repeatWinRegData.ContainsKey(key))
                    {
                        regRepeatWinCount += repeatWinRegData[key];
                    }

                    rowRepeatWin.Add(regRepeatWinCount);

                }

                key = key.AddDays(1);
            }


            return new {
                RowReg = rowReg.ToArray(),
                RowRepeatReg = rowRepeatReg.ToArray(),
                RowRepeatWin = rowRepeatWin.ToArray(),
                Labels = labels.ToArray()
            };
        }
    }
}
