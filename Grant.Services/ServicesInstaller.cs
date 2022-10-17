namespace Grant.Services
{
    using Core.Entities;
    using DomainService;
    using DomainService.TelegramCommands;
    using Install;
    using Utils.Extensions;

    public class ServicesInstaller : BaseInstaller
    {
        public override void Install()
        {
            this.Container.RegisterTransient<IUniversityService, UniversityService>();
            this.Container.RegisterTransient<IStudentService, StudentService>();
            this.Container.RegisterTransient<IGrantEventService, GrantEventService>();
            this.Container.RegisterTransient<IGrantService, GrantService>();
            this.Container.RegisterTransient<IPersonalInfoService, PersonalInfoService>();

            this.Container.RegisterTransient<IGrantQuotaService, GrantQuotaService>();

            this.Container.RegisterTransient<IAchievementService, AchievementService>();
            this.Container.RegisterTransient<IDomainService<Achievement>, AchievementService>();
            this.Container.RegisterTransient<IRecordBookDataService, RecordBookDataService>();
            this.Container.RegisterTransient<IIncomeDataService, IncomeDataService>();

            this.Container.RegisterTransient<IGrantStudentService, GrantStudentService>();
            this.Container.RegisterTransient<IGrantAdminService, GrantAdminService>();


            this.Container.RegisterTransient<IEventService, EventService>();
            this.Container.RegisterTransient<IRoleService, RoleService>();

            this.Container.RegisterTransient<IAchievementScore, AchievementScore>();

            this.Container.RegisterTransient<IBankFilialService, BankFilialService>();


            this.Container.RegisterTransient<IDomainService<PersonalInfo>, BaseDomainService<PersonalInfo>>();
            this.Container.RegisterTransient<IDomainService<GrantFileInfo>, BaseDomainService<GrantFileInfo>>();
            this.Container.RegisterTransient<IDomainService<PersonalInfoHistory>, BaseDomainService<PersonalInfoHistory>>();

            this.Container.RegisterTransient<Core.Services.ITelegramService, TelegramService>();

            this.Container.RegisterTransient<ITelegramCommand, UserSearchCommand>("user");
            this.Container.RegisterTransient<ITelegramCommand, GetAccessCommand>("getaccess");
            this.Container.RegisterTransient<ITelegramCommand, UserDeleteCommand>("deleteuser");


        }
    }
}