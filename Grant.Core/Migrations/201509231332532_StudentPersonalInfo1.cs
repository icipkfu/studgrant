namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPersonalInfo1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "PersonalInfo_Id", "dbo.PersonalInfoes");
            DropIndex("dbo.Students", new[] { "PersonalInfo_Id" });
            RenameColumn(table: "dbo.Students", name: "PersonalInfo_Id", newName: "PersonalInfoId");
            AlterColumn("dbo.Students", "PersonalInfoId", c => c.Long(nullable: false));
            CreateIndex("dbo.Students", "PersonalInfoId");
            AddForeignKey("dbo.Students", "PersonalInfoId", "dbo.PersonalInfoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "PersonalInfoId", "dbo.PersonalInfoes");
            DropIndex("dbo.Students", new[] { "PersonalInfoId" });
            AlterColumn("dbo.Students", "PersonalInfoId", c => c.Long());
            RenameColumn(table: "dbo.Students", name: "PersonalInfoId", newName: "PersonalInfo_Id");
            CreateIndex("dbo.Students", "PersonalInfo_Id");
            AddForeignKey("dbo.Students", "PersonalInfo_Id", "dbo.PersonalInfoes", "Id");
        }
    }
}
