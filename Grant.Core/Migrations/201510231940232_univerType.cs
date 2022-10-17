namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class univerType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Universities", "UniverType", c => c.Int(nullable: false, defaultValue:1));
            AddColumn("dbo.PersonalInfoes", "PassportPage7Id", c => c.Long());
            CreateIndex("dbo.PersonalInfoes", "PassportPage7Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage7Id", "dbo.GrantFileInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage7Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage7Id" });
            DropColumn("dbo.PersonalInfoes", "PassportPage7Id");
            DropColumn("dbo.Universities", "UniverType");
        }
    }
}
