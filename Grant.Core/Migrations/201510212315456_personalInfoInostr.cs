namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class personalInfoInostr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "PassportPage6Id", c => c.Long());
            CreateIndex("dbo.PersonalInfoes", "PassportPage6Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage6Id", "dbo.GrantFileInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage6Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage6Id" });
            DropColumn("dbo.PersonalInfoes", "PassportPage6Id");
        }
    }
}
