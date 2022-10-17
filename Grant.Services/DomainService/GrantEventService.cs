using System.Collections.Generic;
using System.Text;

namespace Grant.Services.DomainService
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Context;
    using Core.Entities;
    using Core.Enum;
    using DataAccess;
    using Utils.Extensions;

    public class GrantEventService : BaseDomainService<GrantEvent>, IGrantEventService
    {
        private readonly IFileManager _fileManager;
        private readonly IStudentService _studentService;
        private readonly IUniversityService _universityService;

        /// <summary>
        /// Список общедоступных событий
        /// </summary>
        private readonly List<EventType> _publicEvents;

        /// <summary>
        /// Список событий по умолчанию 
        /// </summary>
        private readonly List<EventType> _defaultEvents;

        public GrantEventService(IRepository<GrantEvent> repository, IFileManager fileManager,
            IStudentService studentService,
            IUniversityService universityService)
            : base(repository)
        {
            _fileManager = fileManager;
            _studentService = studentService;
            _universityService = universityService;
            _publicEvents = new List<EventType>()
            {
                EventType.GrantOpenRegistration,
                EventType.GrantFinishOpen,
                EventType.GrantOpenDelivery,
                EventType.GrantOpenWinnersSelection
            };

            _defaultEvents = new List<EventType>()
            {
                EventType.GrantOpenRegistration,
                EventType.GrantFinishOpen,
                EventType.GrantOpenDelivery,
                EventType.GrantOpenWinnersSelection
            };
        }

        public override async Task<DataResult> Create(GrantEvent entity)
        {
            await base.Create(entity);

            return DataResult.Ok(entity);
        }

        private async Task<GrantEvent> GenerateDefaultEvent(Grant grant, string title, string subtitle,
            EventType eventType)
        {
            var content = @"<img src='assets/images/backgrounds/bg-1.jpg'/>";

            if (!String.IsNullOrEmpty(grant.ImageFile))
            {
                var image = await _fileManager.Get(grant.ImageFile);

                if (image != null)
                {
                    content = String.Format(@"<img src='{0}'/>", image.VirtualPath);
                }
            }

            var avatar = @"../../assets/images/avatars/hair-black-eyes-blue-green-skin-tanned.png";

            var curUSer = ApplicationContext.Current.CurUserId();

            var student = await _studentService.GetAll().SingleOrDefaultAsync(x => x.UserIdentityId == curUSer);

            if (student != null)
            {
                var image = await _fileManager.Get(student.ImageFile);

                if (image != null)
                {
                    avatar = image.VirtualPath;
                }
            }

            var newEvent = new GrantEvent
            {
                EventDate = DateTime.Now,
                Title = title,
                Subtitle = subtitle,
                Image = avatar,
                Name = grant.Name,
                Content = content,
                GrantId = grant.Id,
                EventType = eventType,
                StudentId = student == null ? (await _studentService.GetCurrent()).Id : student.Id
            };

            return newEvent;
        }

        public async Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType)
        {
            var newEvent = await GenerateDefaultEvent(grant, title, subtitle, eventType);
            await Create(newEvent);
        }

        public async Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType,
            GrantQuota[] changes)
        {
            var newEvent = await GenerateDefaultEvent(grant, title, subtitle, eventType);

            var univDict = _universityService.GetAll().ToDictionary(x => x.Id, y => y.Name);

            var sb = new StringBuilder();

            foreach (var quota in changes)
            {
                sb.AppendFormat("{0} - {1}<br>", univDict[quota.UniversityId], quota.Quota);
            }

            newEvent.QuotaChanged = sb.ToString();

            await Create(newEvent);
        }


        public string GetCriterionName(AchievementCriterion? val)
        {
            if (val == null)
            {
                return "''";
            }

            switch (val)
            {

                case  AchievementCriterion.Award: return "Получение студентом награды(приза)";
                case AchievementCriterion.EventMaster: return "Проведение  публичной культурно-творческой деятельности";
                case AchievementCriterion.GrantResearchWork: return "Получение студентом гранта на выполнение научно-исследовательской работы";
                case AchievementCriterion.Gto: return "Выполнение нормативов и требований ГТО";
                case AchievementCriterion.Patent: return "Получение студентом патента на изобретение";

                case AchievementCriterion.ScientificPublication: return "Наличие у студента публикации в научном издании в течение года";
                case AchievementCriterion.ScientificPublicationRu: return "Наличие у студента публикации в научном издании в течение года во всероссийских изданиях (ВАК, РИНЦ)";
                case AchievementCriterion.ScientificPublicationWorld: return "Наличие у студента публикации в научном издании в течение года в международных изданиях (Scopus, Web of Science, ERIH PLUS";
                case AchievementCriterion.SportEvent: return "Проведение  спортивных мероприятий";
                default: return "";
            }
        }

        public string GetSubjectName(AchievementSubject val)
        {

            switch ((int)val)
            {

                case 0: return "Научно-исследовательская деятельность";
                case 1: return "Спортивная деятельность";
                case 2: return "Культурно творческая деятельность";
                case 3: return "Общественная деятельность";
                case 4: return "Государственные награды, знаки отличия и иные формы поощрения";
                default: return "";

            }
        }

        public string GetLevelName(AchievementLevel? val)
        {
            if(val == null)
            {
                return "''";
            }


            switch ((int)val)
            {

                case 0: return "Международный уровень";
                case 1: return "Всероссийский уровень";
                case 2: return "Региональный уровень";
                case 3: return "Муниципальный (городской)";
                case 4: return "Образовательная организация";

                case 5: return "Руководитель комитета города (муниципального образования)";
                case 6: return "Мэр города (главы муниципального образования)";
                case 7: return "Глава ведомства региона";
                case 8: return "Глава правительства региона";
                case 9: return "Председатель законодательного собрания региона";
                case 10: return "Глава региона";
                case 11: return "Президент республики";
                case 12: return "Президент";
                default: return "";
            }
         }

        public string getStateName(AchievementState? val)
        {

            if (val == null)
            {
                return "''";
            }

            switch ((int)val)
            {

                case 0: return "Победитель";
                case 1: return "Участник";
                case 2: return "Организатор";
                case 3: return "Призер";

                case 4: return "Золото";
                case 5: return "Серебро";
                case 6: return "Бронза";
                default: return "";
             }

        }

        public async Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType, Achievement oldAchievement, Achievement newAchievement)
        {
            var newEvent = await GenerateDefaultEvent(grant, title, subtitle, eventType);

            var sb = new StringBuilder();

            if (newAchievement.Name != oldAchievement.Name)
            {
                sb.Append($"Наименование было: {oldAchievement.Name}, cтало: {newAchievement.Name} <br>");
            }

            if (newAchievement.Subject != oldAchievement.Subject)
            {
                sb.Append($"Область деятельности была: {GetSubjectName(oldAchievement.Subject)}, cтала: {GetSubjectName(newAchievement.Subject)} <br>");
            }


            if (newAchievement.Level != oldAchievement.Level)
            {
                sb.Append($"Уровень был: {GetLevelName(oldAchievement.Level)}, cтал: {GetLevelName(newAchievement.Level)} <br>");
            }

            if (newAchievement.Criterion != oldAchievement.Criterion)
            {
                sb.Append($"Критерий был: {GetCriterionName(oldAchievement.Criterion)}, cтал: {GetCriterionName(newAchievement.Criterion)} <br>");
            }

            if (newAchievement.State != oldAchievement.State)
            {
                sb.Append($"Статус был: {getStateName(oldAchievement.State)}, cтал: {getStateName(newAchievement.State)} <br>");
            }

            if (newAchievement.Year != oldAchievement.Year)
            {
                sb.Append($"Год был: {oldAchievement.Year}, cтал: {newAchievement.Year} <br>");
            }


            if(newAchievement.Files != oldAchievement.Files)
            {
                var oldLink = "Было:";
                var newLink = "Стало:";

                if (!newAchievement.Files.IsEmpty())
                {
                    var imageNew = await _fileManager.Get(newAchievement.Files);
                
                     newLink = $"<a href={imageNew.VirtualPath} target='_blank'> Стало </a>";
                }

                if (!oldAchievement.Files.IsEmpty())
                {
                    var imageOld = await _fileManager.Get(oldAchievement.Files);

                    oldLink = $"<a href={imageOld.VirtualPath} target='_blank' > Было </a>";
                }

                sb.Append($"Изображение {oldLink}, {newLink} <br>");
            }

            newEvent.Subtitle += sb.ToString();
            newEvent.StudentId = newAchievement.StudentId;


            await Create(newEvent);
        }

        public async Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType, Grant oldGrant)
        {
            var newEvent = await GenerateDefaultEvent(grant, title, subtitle, eventType);

            if (oldGrant == null)
            {
                newEvent.Description = grant.Description;
                newEvent.Conditions = grant.Conditions;
                newEvent.DateChange = grant.ExpiresDate.ToShortDateString();
                newEvent.ChangeAdmin = grant.Administrators;

                await Create(newEvent);
                return;
            }

            var newDescription = grant.Description;
            var oldDescription = oldGrant.Description;

            var newConditions = oldGrant.Conditions;
            var oldConditions = grant.Conditions;

            var newAdmin = grant.Administrators;
            var oldAdmin = oldGrant.Administrators;

            if (string.Compare(oldDescription, newDescription, StringComparison.CurrentCulture) != 0)
            {
                newEvent.Description = newDescription.Replace(oldDescription, "<..>");
            }

            if (string.Compare(oldConditions, newConditions, StringComparison.CurrentCulture) != 0)
            {
                newEvent.Conditions = newConditions.Replace(oldConditions, "<..>");
            }

            if (grant.ExpiresDate != oldGrant.ExpiresDate)
            {
                newEvent.DateChange = grant.ExpiresDate.ToShortDateString();
            }

            if (String.Compare(grant.Administrators, oldGrant.Administrators, StringComparison.CurrentCulture) != 0)
            {
                if (!String.IsNullOrEmpty(grant.Administrators))
                {
                    var adminsId = grant.Administrators.Split(',');
                    var sb = new StringBuilder();
                    foreach (var id in adminsId)
                    {
                        var student = await _studentService.Get(id.ToLong());
                        if (student != null)
                        {
                            sb.Append(String.Format("{0} {1} {2} <br>", student.Name, student.LastName,
                                student.Patronymic));
                        }
                    }
                    newEvent.ChangeAdmin = sb.ToString();
                }
                else
                {
                    newEvent.ChangeAdmin = "Администраторы гранта были удалены";
                }

            }
            await Create(newEvent);
        }

        public Task CreateEvent(Grant grant, string title, string subtitle, string image, string content, string palette,
            string classes)
        {
            throw new NotImplementedException();
        }


        public async Task<IQueryable<GrantEvent>> GetGrantEvents(long id, EventFilter filter, List<EventType> eventTypes = null)
        {


            var nameArr = filter.Search != null ? filter.Search.Split(' ') : new string[] { string.Empty };

            var name = nameArr[0].ToUpper();
            var lastName = nameArr.Length > 1 ? nameArr[1].ToUpper() : null;
            var patronymic = nameArr.Length > 2 ? nameArr[2].ToUpper() : null;

            var roleInfo = await _studentService.GetCurrentRole();

            if (roleInfo.Role == Role.Administrator)
            {
                return base.GetAll()           
                    .WhereIf(filter.LastId > 0, x => x.Id < filter.LastId)
                    .Where(x => x.GrantId == id)
                    .WhereIf(!string.IsNullOrEmpty(name), x=>x.Subtitle.ToUpper().Contains(name))
                    .WhereIf(!string.IsNullOrEmpty(lastName), x => x.Subtitle.ToUpper().Contains(lastName))
                    .WhereIf(!string.IsNullOrEmpty(patronymic), x => x.Subtitle.ToUpper().Contains(patronymic))
                    .WhereIf(filter.Type.HasValue, x => x.EventType == (EventType) filter.Type.Value)
                    .WhereIf(filter.Before.HasValue, x=>x.EventDate <= filter.Before.Value)
                    .WhereIf(filter.After.HasValue, x => x.EventDate >= filter.After.Value)
                    .OrderByDescending(x => x.EventDate)
                    .Take(10);
            }
            
            //старый метод получения событий
            return base.GetAll()
                .Where(x => x.GrantId == id)
                 .WhereIf(!string.IsNullOrEmpty(name), x => x.Subtitle.ToUpper().Contains(name))
                 .WhereIf(!string.IsNullOrEmpty(lastName), x => x.Subtitle.ToUpper().Contains(lastName))
                 .WhereIf(!string.IsNullOrEmpty(patronymic), x => x.Subtitle.ToUpper().Contains(patronymic))
                .WhereIf(filter.LastId > 0, x => x.Id < filter.LastId)
                .WhereIf(filter.Type.HasValue, x => x.EventType == (EventType)filter.Type.Value)
                .WhereIf(filter.Before.HasValue, x => x.EventDate <= filter.Before.Value)
                .WhereIf(filter.After.HasValue, x => x.EventDate >= filter.After.Value)
                .Where(x => roleInfo.GrantsAdmin.Contains(x.GrantId) || _publicEvents.Contains(x.EventType)
                            || x.StudentId == roleInfo.Id)
                .OrderByDescending(x => x.EventDate).Take(10);
            
            //новый метод для получения событий по запрошенному типу
            var displayedEventTypes = eventTypes ?? _defaultEvents;

            return base.GetAll()
                .Where(x => x.GrantId == id)
                .Where(x => displayedEventTypes.Contains(x.EventType) && 
                    (roleInfo.GrantsAdmin.Contains(x.GrantId) || _publicEvents.Contains(x.EventType) || x.StudentId == roleInfo.Id))
                .OrderByDescending(x => x.EventDate);
        }


        public async Task<IQueryable<GrantEvent>> GetGrantEvents(EventFilter filter, List<EventType> eventTypes = null)
        {
            var nameArr = filter.Search != null ? filter.Search.Split(' ') : new string[] { string.Empty };

            var name = nameArr[0].ToUpper();
            var lastName = nameArr.Length > 1 ? nameArr[1].ToUpper() : null;
            var patronymic = nameArr.Length > 2 ? nameArr[2].ToUpper() : null;

            var roleInfo = await _studentService.GetCurrentRole();

            if (roleInfo.Role == Role.Administrator)
            {
                return base.GetAll()
                    .WhereIf(filter.LastId > 0, x => x.Id < filter.LastId)
                    .WhereIf(!string.IsNullOrEmpty(name), x => x.Subtitle.ToUpper().Contains(name))
                    .WhereIf(!string.IsNullOrEmpty(lastName), x => x.Subtitle.ToUpper().Contains(lastName))
                    .WhereIf(!string.IsNullOrEmpty(patronymic), x => x.Subtitle.ToUpper().Contains(patronymic))
                    .WhereIf(filter.Type.HasValue, x => x.EventType == (EventType)filter.Type.Value)
                    .WhereIf(filter.Before.HasValue, x => x.EventDate <= filter.Before.Value)
                    .WhereIf(filter.After.HasValue, x => x.EventDate >= filter.After.Value)
                    .OrderByDescending(x => x.EventDate).Take(10);
            }

            //старый метод получения событий
            return base.GetAll().Include(x => x.Grant)
                .WhereIf(filter.LastId > 0, x => x.Id < filter.LastId)
                .WhereIf(!string.IsNullOrEmpty(name), x => x.Subtitle.ToUpper().Contains(name))
                .WhereIf(!string.IsNullOrEmpty(lastName), x => x.Subtitle.ToUpper().Contains(lastName))
                .WhereIf(!string.IsNullOrEmpty(patronymic), x => x.Subtitle.ToUpper().Contains(patronymic))
                .WhereIf(filter.Type.HasValue, x => x.EventType == (EventType)filter.Type.Value)
                .WhereIf(filter.Before.HasValue, x => x.EventDate <= filter.Before.Value)
                .WhereIf(filter.After.HasValue, x => x.EventDate >= filter.After.Value)
                .Where(x => roleInfo.GrantsAdmin.Contains(x.GrantId) || _publicEvents.Contains(x.EventType)
                            || x.StudentId == roleInfo.Id)
                             .OrderByDescending(x => x.EventDate).Take(10);
            
            //новый метод для получения событий по запрошенному типу
            var displayedEventTypes = eventTypes ?? _defaultEvents;

            return base.GetAll()
                .Include(x => x.Grant)
                .Where(x => displayedEventTypes.Contains(x.EventType) &&
                            (roleInfo.GrantsAdmin.Contains(x.GrantId) || _publicEvents.Contains(x.EventType) || x.StudentId == roleInfo.Id))
                .OrderByDescending(x => x.EventDate);
        }
    }
}
