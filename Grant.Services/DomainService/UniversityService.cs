using System;
using System.Collections.Generic;
using System.Linq;
using Grant.Utils.Extensions;

namespace Grant.Services.DomainService
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core;
    using Core.Entities;
    using DataAccess;

    class UniversityService : BaseDomainService<University>, IUniversityService
    {

        private IFileManager fileManager;
        private IStudentService studentService;
        private IBankFilialService bankService;

        public UniversityService(IRepository<University> repository, IFileManager fileManager, IStudentService studentService, IBankFilialService bankService) : base(repository)
        {
            this.fileManager = fileManager;
            this.studentService = studentService;
            this.bankService = bankService;
        }

        public async override Task<University> Get(long id)
        {
            var entity = await base.Get(id);

            if (entity != null)
            {
                if (!String.IsNullOrEmpty(entity.ImageFile))
                {
                    var image = await fileManager.Get(entity.ImageFile);

                    if (image != null)
                    {
                        entity.ImageLink = image.VirtualPath;
                        entity.thumb = image.VirtualPath.Replace("\\", "/");
                    }
                }
                else
                {
                    entity.ImageLink = "../../../assets/images/grant/unviersity_default.png";
                    entity.thumb = "../../../assets/images/grant/unviersity_default.png";
                }


                if (entity.CuratorId > 0)
                {
                    entity.Curator =
                        await studentService.GetAll().Where(x => x.Id == entity.CuratorId).SingleOrDefaultAsync();

                    if (entity.Curator != null)
                    {
                        var image = await fileManager.Get(entity.Curator.ImageFile);

                        if (image != null)
                        {
                            entity.Curator.ImageLink = image.VirtualPath;
                            entity.Curator.thumb = image.VirtualPath.Replace("\\", "/");
                        }
                    }
                }


                if (entity.BankFilialId > 0)
                {
                    entity.BankFilial =
                        await bankService.GetAll().Where(x => x.Id == entity.BankFilialId).SingleOrDefaultAsync();
              
                }
            }

            return entity;
        }

        public async override Task<IQueryable<University>> GetAllAsync()
        {

            var studentStat = await
                studentService.GetAll()
                    .Where(x => x.UniversityId > 0)
                    .GroupBy(x => x.UniversityId)
                    .ToDictionaryAsync(x => x.Key, y => y.Count());

            var curatorsId = await GetAll().Where(x => x.CuratorId > 0).Select(x => x.CuratorId).Distinct().ToListAsync();

            var curators = await studentService.GetAll()
                .Where(x => curatorsId.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, y => y);

            var roleInfo = await studentService.GetCurrentRole();

            var isAdmin = roleInfo.Role == Role.Administrator || roleInfo.GrantsAdmin.Any();

            var result = await base.GetAll().Where(x => isAdmin || roleInfo.UniversCurator.Contains(x.Id)).OrderBy(x => x.Name).ToListAsync();

            foreach (var univer in result)
            {
                //if (!String.IsNullOrEmpty(univer.ImageFile))
                //{
                //    var image = await fileManager.Get(univer.ImageFile);

                //    if (image != null)
                //    {
                //        univer.ImageLink = image.VirtualPath;
                //        univer.thumb = univer.ImageLink.Replace("\\", "/");
                //    }
                //}
                //else
                //{
                //    univer.thumb = "../assets/images/grant/unviersity_default.png";
                //}

                univer.RegisteredCount = studentStat.ContainsKey(univer.Id) ? studentStat[univer.Id] : 0;

                univer.thumb = "../assets/images/grant/unviersity_default.png";

                var studId = univer.CuratorId;

                if (studId != 0)
                {
                    var curator = curators.ContainsKey(studId) ? curators[studId] : null;

                    if (curator != null)
                    {
                        univer.CuratorFio = string.Format("{0} {1} {2}", curator.Name, curator.LastName, curator.Patronymic);
                    }
                }
            }

            return result.AsQueryable();
        }


        public async Task<IQueryable<University>> GetAllUniversities(UniversityFilter filter)
        {

            if (filter == null)
            {
                filter = new UniversityFilter();
            }

            var curatorsId = await GetAll().Where(x => x.CuratorId > 0).Select(x => x.CuratorId).Distinct().ToListAsync();

            var curators = await studentService.GetAll()
                .Where(x => curatorsId.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, y => y);

            var roleInfo = await studentService.GetCurrentRole();

            var isAdmin = roleInfo.Role == Role.Administrator || roleInfo.Role ==  Role.ZoneModerator || roleInfo.GrantsAdmin.Any();

            var result = await base.GetAll()
                .Where(x => isAdmin || roleInfo.UniversCurator.Contains(x.Id))
                .WhereIf(!string.IsNullOrEmpty(filter.Search) && filter.Search.Length > 0, x=>x.Name.ToLower().Contains(filter.Search.ToLower()) || x.Town.ToLower().Contains(filter.Search.ToLower()))
                .WhereIf(filter.Type.HasValue, x=>(int) x.UniverType == filter.Type.Value)
                .OrderBy(x => x.Name).ToListAsync();

            foreach (var univer in result)
            {
                //if (!String.IsNullOrEmpty(univer.ImageFile))
                //{
                //    var image = await fileManager.Get(univer.ImageFile);

                //    if (image != null)
                //    {
                //        univer.ImageLink = image.VirtualPath;
                //        univer.thumb = univer.ImageLink.Replace("\\", "/");
                //    }
                //}
                //else
                //{
                //    univer.thumb = "../assets/images/grant/unviersity_default.png";
                //}

                univer.thumb = "../assets/images/grant/unviersity_default.png";

                var studId = univer.CuratorId;

                if (studId != 0)
                {
                    var curator = curators.ContainsKey(studId) ? curators[studId] : null;

                    if (curator != null)
                    {
                        univer.CuratorFio = string.Format("{0} {1} {2}", curator.Name, curator.LastName, curator.Patronymic);
                    }
                }
            }

            return result.AsQueryable();
        }

        public async override Task<DataResult> Create(University university)
        {
            if (await GetAll().AnyAsync(x => x.Name == university.Name))
            {
                return DataResult.Failure("Данный ВУЗ уже зарегистрирован в системе");
            }

            //if (await GetAll().AnyAsync(x => x.Curator == university.Curator))
            //{
            //    return DataResult.Failure("Данный куратор прикреплен за другим ВУЗом");
            //}

            return await base.Create(university);
        }
    }
}
