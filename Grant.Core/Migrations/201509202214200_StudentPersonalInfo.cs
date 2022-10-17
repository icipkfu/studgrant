namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPersonalInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonalInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Citizenship = c.Int(nullable: false),
                        Phone = c.String(),
                        Birthday = c.DateTime(),
                        Birthplace = c.String(),
                        PassportSeries = c.String(),
                        PassportNumber = c.String(),
                        PassportIssueDate = c.DateTime(),
                        PassportIssuedBy = c.String(),
                        PassportIssuedByCode = c.String(),
                        RegistrationIndex = c.String(),
                        RegistrationCity = c.String(),
                        RegistrationStreet = c.String(),
                        RegistrationHouse = c.String(),
                        RegistrationHousing = c.String(),
                        RegistrationFlat = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Achievements", "Subject", c => c.Int(nullable: false));
            AddColumn("dbo.Achievements", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Achievements", "Level", c => c.Int(nullable: false));
            AddColumn("dbo.Achievements", "Year", c => c.Int(nullable: false));
            AddColumn("dbo.Achievements", "Student_Id", c => c.Long());
            AddColumn("dbo.Students", "PersonalInfo_Id", c => c.Long());
            CreateIndex("dbo.Achievements", "Student_Id");
            CreateIndex("dbo.Students", "PersonalInfo_Id");
            AddForeignKey("dbo.Students", "PersonalInfo_Id", "dbo.PersonalInfoes", "Id");
            AddForeignKey("dbo.Achievements", "Student_Id", "dbo.Students", "Id");
            DropColumn("dbo.Achievements", "Description");
            DropColumn("dbo.Achievements", "Description1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Achievements", "Description1", c => c.String());
            AddColumn("dbo.Achievements", "Description", c => c.String());
            DropForeignKey("dbo.Achievements", "Student_Id", "dbo.Students");
            DropForeignKey("dbo.Students", "PersonalInfo_Id", "dbo.PersonalInfoes");
            DropIndex("dbo.Students", new[] { "PersonalInfo_Id" });
            DropIndex("dbo.Achievements", new[] { "Student_Id" });
            DropColumn("dbo.Students", "PersonalInfo_Id");
            DropColumn("dbo.Achievements", "Student_Id");
            DropColumn("dbo.Achievements", "Year");
            DropColumn("dbo.Achievements", "Level");
            DropColumn("dbo.Achievements", "State");
            DropColumn("dbo.Achievements", "Subject");
            DropTable("dbo.PersonalInfoes");
        }
    }
}
