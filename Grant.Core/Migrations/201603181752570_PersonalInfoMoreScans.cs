namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoMoreScans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "PassportPage9Id", c => c.Long());
            AddColumn("dbo.PersonalInfoes", "PassportPage10Id", c => c.Long());
            CreateIndex("dbo.PersonalInfoes", "PassportPage9Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage10Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage10Id", "dbo.GrantFileInfoes", "Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage9Id", "dbo.GrantFileInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage9Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage10Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage10Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage9Id" });
            DropColumn("dbo.PersonalInfoes", "PassportPage10Id");
            DropColumn("dbo.PersonalInfoes", "PassportPage9Id");
        }
    }
}
