namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityUserIdField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "UserId", c => c.Long());
            AddColumn("dbo.Students", "UserId", c => c.Long());
            AddColumn("dbo.DataFiles", "UserId", c => c.Long());
            AddColumn("dbo.GrantFileInfoes", "UserId", c => c.Long());
            AddColumn("dbo.GrantStudents", "UserId", c => c.Long());
            AddColumn("dbo.Grants", "UserId", c => c.Long());
            AddColumn("dbo.CompetitionConditions", "UserId", c => c.Long());
            AddColumn("dbo.Universities", "UserId", c => c.Long());
            AddColumn("dbo.PersonalInfoes", "UserId", c => c.Long());
            AddColumn("dbo.PersonalInfoFiles", "UserId", c => c.Long());
            AddColumn("dbo.Portfolios", "UserId", c => c.Long());
            AddColumn("dbo.Events", "UserId", c => c.Long());
            AddColumn("dbo.GrantAdmins", "UserId", c => c.Long());
            AddColumn("dbo.GrantQuotas", "UserId", c => c.Long());
            AddColumn("dbo.NotificationQueues", "UserId", c => c.Long());
            AddColumn("dbo.Status", "UserId", c => c.Long());
            AddColumn("dbo.UniversityCurators", "UserId", c => c.Long());
            AddColumn("dbo.ValidationHistories", "UserId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ValidationHistories", "UserId");
            DropColumn("dbo.UniversityCurators", "UserId");
            DropColumn("dbo.Status", "UserId");
            DropColumn("dbo.NotificationQueues", "UserId");
            DropColumn("dbo.GrantQuotas", "UserId");
            DropColumn("dbo.GrantAdmins", "UserId");
            DropColumn("dbo.Events", "UserId");
            DropColumn("dbo.Portfolios", "UserId");
            DropColumn("dbo.PersonalInfoFiles", "UserId");
            DropColumn("dbo.PersonalInfoes", "UserId");
            DropColumn("dbo.Universities", "UserId");
            DropColumn("dbo.CompetitionConditions", "UserId");
            DropColumn("dbo.Grants", "UserId");
            DropColumn("dbo.GrantStudents", "UserId");
            DropColumn("dbo.GrantFileInfoes", "UserId");
            DropColumn("dbo.DataFiles", "UserId");
            DropColumn("dbo.Students", "UserId");
            DropColumn("dbo.Achievements", "UserId");
        }
    }
}
