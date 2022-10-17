using Grant.Core.Context;

namespace Grant.Core.DbContext
{
    using System.Data.Entity;
    //  using Areas.Domain.EvensArgs;
    using Grant.Core.Entities;
    using System;
    using Grant.Core.Interfaces;
    using System.Linq;
    using AspNet.Identity.PostgreSQL;

    //public delegate void EntitySaveHandler(EntitySaveEvenArgs args);

    public class GrantDbContext : PostgreSQLDatabase 
    {

        public GrantDbContext()
        {
            Database.SetInitializer<GrantDbContext>(null);
        }

        public static GrantDbContext Create()
        {
            return new GrantDbContext();
        }

//        private IDateTimeProvider timeProvider;

       // public event EntitySaveHandler EntitySaving;

        public DbSet<GrantStudent> GrantStudents { get; set; }

        public DbSet<GrantEvent> GrantEvents { get; set; }

        public DbSet<GrantQuota> GrantQuotas { get; set; } 

        public DbSet<Achievement> Achievements { get; set; }

        public DbSet<CompetitionCondition> CompetitionConditions { get; set; }

        public DbSet<DataFile> DataFiles { get; set; }
        
        public DbSet<Event> Events { get; set; }

        public DbSet<Grant> Grants { get; set; }

        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<UniversityCurator> UniversityCurator { get; set; }

        //public DbSet<Role> Roles { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<University> Universities { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<IdentityUser> Users { get; set; }

        public DbSet<GrantFileInfo> FilesInfo { get; set; }

        public DbSet<PersonalInfoFile> PersonalInfoFiles { get; set; }

        public DbSet<GrantAdmin> GrantAdmin { get; set; }

        public DbSet<NotificationQueue> NotificationQueue { get; set; }

        public DbSet<ValidationHistory> ValidationHistory { get; set; }

        public DbSet<PersonalInfoHistory> PersonalInfoHistory { get; set; }

        public DbSet<BankFilial> BankFilial { get; set; }

        public DbSet<TelegramUser> TelegramUser { get; set; }

        public DbSet<Settings> Settings { get; set; }


        public override async System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            var now = DateTime.Now;

            var curUSer = ApplicationContext.Current.CurUserId();

            var user = await Students.SingleOrDefaultAsync(x => x.UserIdentityId == curUSer);

            var addedAuditedEntities =
                this.ChangeTracker.Entries<IBaseEntity>()
                    .Where(x => x.State == EntityState.Added)
                    .Select(x => x.Entity);


            foreach (var added in addedAuditedEntities)
            {
                added.CreateDate = now;
                added.EditDate = now;

                if (user != null)
                {
                    added.UserId = user.Id;
                }
                
            }

            var modifiedAuditedEntities =
                this.ChangeTracker.Entries<IBaseEntity>()
                    .Where(p => p.State == EntityState.Modified)
                    .Select(p => p.Entity);

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.EditDate = now;

                if (user != null)
                {
                    modified.UserId = user.Id;
                }
            }

            return  await base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            var now = DateTime.UtcNow;

            var addedAuditedEntities =
                this.ChangeTracker.Entries<IBaseEntity>()
                    .Where(x => x.State == EntityState.Added)
                    .Select(x => x.Entity);


            foreach (var added in addedAuditedEntities)
            {
                added.CreateDate = now;
                added.EditDate = now;
            }

            var modifiedAuditedEntities =
                this.ChangeTracker.Entries<IBaseEntity>()
                    .Where(p => p.State == EntityState.Modified)
                    .Select(p => p.Entity);

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.EditDate = now;
            }

            //var savingEntity = addedAuditedEntities.SingleOrDefault();

            //EntitySaveHandler onEntitySaving = this.EntitySaving;
            //if (onEntitySaving != null)
            //{
            //    onEntitySaving(new EntitySaveEvenArgs
            //    {
            //        Id = savingEntity.Id,
            //        Name = savingEntity.Name
            //    });

            //}

            return base.SaveChanges();
        }

    }
}