namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class persInfohistory3 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.PersonalInfoHistories",
            //    c => new
            //        {
            //            Id = c.Long(nullable: false, identity: true),
            //            Sex = c.Int(nullable: false),
            //            IsLiveAddressSame = c.Boolean(nullable: false),
            //            Citizenship = c.Int(nullable: false),
            //            Phone = c.String(),
            //            Inn = c.String(),
            //            Birthday = c.DateTime(),
            //            Birthplace = c.String(),
            //            PassportSeries = c.String(),
            //            PassportNumber = c.String(),
            //            PassportIssueDate = c.DateTime(),
            //            PassportIssuedBy = c.String(),
            //            PassportIssuedByCode = c.String(),
            //            PassportScan = c.String(),
            //            Agreement = c.Boolean(nullable: false),
            //            PassportPage1Id = c.Long(),
            //            PassportPage2Id = c.Long(),
            //            PassportPage3Id = c.Long(),
            //            PassportPage4Id = c.Long(),
            //            PassportPage5Id = c.Long(),
            //            PassportPage6Id = c.Long(),
            //            PassportPage7Id = c.Long(),
            //            PassportPage8Id = c.Long(),
            //            PassportPage9Id = c.Long(),
            //            PassportPage10Id = c.Long(),
            //            RegistrationRepublic = c.String(),
            //            RegistrationzZone = c.String(),
            //            RegistrationIndex = c.String(),
            //            RegistrationCity = c.String(),
            //            RegistrationPlace = c.String(),
            //            RegistrationStreet = c.String(),
            //            RegistrationHouse = c.String(),
            //            RegistrationHousing = c.String(),
            //            RegistrationFlat = c.String(),
            //            LiveRepublic = c.String(),
            //            LiveZone = c.String(),
            //            LiveIndex = c.String(),
            //            LiveCity = c.String(),
            //            LivePlace = c.String(),
            //            LiveStreet = c.String(),
            //            LiveHouse = c.String(),
            //            LiveHousing = c.String(),
            //            LiveFlat = c.String(),
            //            Name = c.String(maxLength: 255),
            //            CreateDate = c.DateTime(nullable: false),
            //            EditDate = c.DateTime(nullable: false),
            //            DeletedMark = c.Boolean(nullable: false),
            //            UserId = c.Long(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage1Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage10Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage2Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage3Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage4Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage5Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage6Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage7Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage8Id)
            //    .ForeignKey("dbo.GrantFileInfoes", t => t.PassportPage9Id)
            //    .Index(t => t.PassportPage1Id)
            //    .Index(t => t.PassportPage2Id)
            //    .Index(t => t.PassportPage3Id)
            //    .Index(t => t.PassportPage4Id)
            //    .Index(t => t.PassportPage5Id)
            //    .Index(t => t.PassportPage6Id)
            //    .Index(t => t.PassportPage7Id)
            //    .Index(t => t.PassportPage8Id)
            //    .Index(t => t.PassportPage9Id)
            //    .Index(t => t.PassportPage10Id);
            
            //AddColumn("dbo.PersonalInfoFiles", "PersonalInfoHistory_Id", c => c.Long());
            //CreateIndex("dbo.PersonalInfoFiles", "PersonalInfoHistory_Id");
            //AddForeignKey("dbo.PersonalInfoFiles", "PersonalInfoHistory_Id", "dbo.PersonalInfoHistories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoFiles", "PersonalInfoHistory_Id", "dbo.PersonalInfoHistories");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage9Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage8Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage7Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage6Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage5Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage4Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage3Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage2Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage10Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoHistories", "PassportPage1Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage10Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage9Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage8Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage7Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage6Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage5Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage4Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage3Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage2Id" });
            DropIndex("dbo.PersonalInfoHistories", new[] { "PassportPage1Id" });
            DropIndex("dbo.PersonalInfoFiles", new[] { "PersonalInfoHistory_Id" });
            DropColumn("dbo.PersonalInfoFiles", "PersonalInfoHistory_Id");
            DropTable("dbo.PersonalInfoHistories");
        }
    }
}
