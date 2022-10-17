namespace Grant.Services.DomainService
{
    using Core.Enum;
    using Core;
    using Core.Entities;
    using DataAccess;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GrantQuotaService : BaseDomainService<GrantQuota>, IGrantQuotaService
    {
        private IRepository<GrantQuota> repository;
        private IGrantEventService eventService;
        private IGrantService grantService;
        private IStudentService studentService;
        private IGrantStudentService grantStudentService;
        private IUniversityService universityService;
        private IFileManager fileManager;

        public GrantQuotaService(IRepository<GrantQuota> repository, IGrantEventService eventService,
            IGrantService grantService, IStudentService studentService, IGrantStudentService grantStudentService,
            IUniversityService universityService, IFileManager fileManager) : base(repository)
        {
            this.repository = repository;
            this.eventService = eventService;
            this.grantService = grantService;
            this.studentService = studentService;
            this.grantStudentService = grantStudentService;
            this.universityService = universityService;
            this.fileManager = fileManager;
        }

        public async Task<List<GrantQuota>> GetGrantQuotas(long grantId)
        {
            var grantPeopleStat =  await grantStudentService.GetAll()
                .Where(x=>x.GrantId == grantId)
                .GroupBy(x => x.UniversityId)
                .ToDictionaryAsync(x => x.Key, y => new
                {
                    count = y.Count(),
                    winners = y.Count(z => z.IsAdditionalWinner || z.IsWinner)
                });


            var result = await GetAll().Where(x => x.GrantId == grantId).ToListAsync();

            foreach (var item in result)
            {
                item.StudentCount = grantPeopleStat.ContainsKey(item.UniversityId)
                    ? grantPeopleStat[item.UniversityId].count
                    : 0;

                item.WinnerCount = grantPeopleStat.ContainsKey(item.UniversityId)
                    ? grantPeopleStat[item.UniversityId].winners
                    : 0;

                if (!string.IsNullOrEmpty(item.WinnerReport))
                {
                    var file = await fileManager.Get(item.WinnerReport);

                    if (file != null)
                    {
                        item.Link = file.VirtualPath;
                    }
                }

                grantPeopleStat.Remove(item.UniversityId);
            }

            result.AddRange(grantPeopleStat.Select(item => new GrantQuota
            {
                UniversityId = item.Key, Quota = 0, GrantId = grantId, StudentCount = item.Value.count, WinnerCount = item.Value.winners
            }));

            return result;

        }

        public async Task<GrantQuota> GetUniversityQuota(long grantId, long univerId)
        {
            var winnerCountDict =
                await
                    grantStudentService.GetAll()
                        .Where(x => x.GrantId == grantId && (x.IsWinner || x.IsAdditionalWinner))
                        .GroupBy(x => x.UniversityId)
                        .ToDictionaryAsync(x => x.Key, y => y.Count());

            var quota =
                await
                    GetAll()
                        .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                        .SingleOrDefaultAsync();

            if (quota != null)
            {
                var winnerCount = winnerCountDict.ContainsKey(quota.UniversityId) ? winnerCountDict[quota.UniversityId] : 0;

                quota.Quota -= winnerCount;
            }
            else
            {
                quota = new GrantQuota();
            }

            return quota;
        }

        public async Task<GrantQuota> GetUniversityFullQuota(long grantId, long univerId)
        {
            var quota =
                await
                    GetAll()
                        .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                        .SingleOrDefaultAsync();

            if (quota != null)
            {
                await grantStudentService.GetAll()
                    .CountAsync(
                        x =>
                            x.GrantId == grantId && x.UniversityId == univerId &&
                            (x.IsWinner || x.IsAdditionalWinner));
            }
            else
            {
                quota = new GrantQuota();
            }

            return quota;
        }

        public async Task Update(long grantId, GrantQuota[] quotas)
        {
            foreach (var quota in quotas)
            {
                if (quota.Id > 0)
                {
                    await repository.Update(quota, true);
                }
                else
                {
                    await repository.Create(quota, true);
                }
            }

            await repository.SaveChangesAsync();

            var grant = await grantService.Get(grantId);

            await
                eventService.CreateEvent(grant, "Изменение квот", "Изменены квоты по ВУЗам", EventType.GrantQuotaChanged,
                    quotas);
        }

        public async Task<DataResult> AddWinnerReport(long grantId,  long univerId, string report)
        {
            var grant = await grantService.Get(grantId);

            if (!grant.CanAddReport)
            {
                return DataResult.Failure("Возможность добавлять отчет о победителях на данный момент закрыта администрацией");
            }

                var quota =
                    await
                        GetAll().Include(x=>x.University).Include(x=>x.Grant)
                            .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                            .SingleOrDefaultAsync();
                if (quota != null)
                {
                    quota.WinnerReport = report;
                    await Update(quota);
                    var university = quota.University;

                    var eventText = string.Format("Загружен отчет о победителях {0}", university.Name);


                    await
                        eventService.CreateEvent(grant, "Загружен отчет о победителях", eventText,
                            EventType.WinnerReportLoaded);

                    return DataResult.Ok(quota);
                }
                else
                {
                    return DataResult.Failure("У Вашего вуза отсутствует квота на данный грант");
                }
        }

        public async Task<DataResult> DeleteWinnerReport(long grantId, long univerId)
        {
            var grant = await grantService.Get(grantId);

            if (!grant.CanAddReport)
            {
                return DataResult.Failure("Возможность удалять отчет о победителях на данный момент закрыта администрацией");
            }

                var quota =
                    await
                        GetAll().Include(x=>x.University).Include(x=>x.Grant)
                            .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                            .SingleOrDefaultAsync();

                if (quota != null)
                {
                    quota.WinnerReport = null;
                    await Update(quota);
                    var university = quota.University;

                    var eventText = string.Format("Удален отчет о победителях {0}", university.Name);
                    await
                        eventService.CreateEvent(grant, "Удален отчет о победителях", eventText,
                            EventType.WinnerReportDeleted);

                    return DataResult.Ok(quota);
                }
                else
                {
                    return DataResult.Failure("У Вашего вуза отсутствует квота на данный грант");
                }
            
        }

        public async Task<DataResult> AddAdditionalWinnerReport(long grantId, long univerId, string report)
        {

            var grant = await grantService.Get(grantId);

            if (!grant.CanAddReport)
            {
                return DataResult.Failure("Возможность добавлять отчет о победителях на данный момент закрыта администрацией");
            }

                var quota =
                    await
                        GetAll().Include(x=>x.University).Include(x=>x.Grant)
                            .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                            .SingleOrDefaultAsync();
                if (quota != null)
                {
                    quota.AdditionalWinnerReport = report;
                    await Update(quota);
                    var university = quota.University;

                    var eventText = string.Format("Загружен отчет о дополнительных победителях {0}", university.Name);


                    await
                        eventService.CreateEvent(grant, "Загружен отчет о дополнительных победителях", eventText,
                            EventType.AdditionalWinnerReportLoaded);

                    return DataResult.Ok(quota);
                }
                else
                {
                    return DataResult.Failure("У Вашего вуза отсутствует квота на данный грант");
                }
            
        }

        public async Task<DataResult> DeleteAdditionalWinnerReport(long grantId, long univerId)
        {
            var grant = await grantService.Get(grantId);

            if (!grant.CanAddReport)
            {
                return DataResult.Failure("Возможность удалять отчет о победителях на данный момент закрыта администрацией");
            }


                var quota =
                    await
                        GetAll().Include(x=>x.University).Include(x=>x.Grant)
                            .Where(x => x.GrantId == grantId && x.UniversityId == univerId)
                            .SingleOrDefaultAsync();
               
            if (quota != null)
                {
                    quota.AdditionalWinnerReport = null;
                    await Update(quota);
                    var university = quota.University;

                    var eventText = string.Format("Удален отчет о дополнительных победителях {0}", university.Name);
                    await
                        eventService.CreateEvent(grant, "Удален отчет о дополнительных победителях", eventText,
                            EventType.AdditionalWinnerReportDeleted);

                    return DataResult.Ok(quota);
                }
                else
                {
                    return DataResult.Failure("У Вашего вуза отсутствует квота на данный грант");
                }
        }
    }
}
