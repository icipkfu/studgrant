namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoPassportFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "PassportPage1Id", c => c.Long(nullable: true));
            AddColumn("dbo.PersonalInfoes", "PassportPage2Id", c => c.Long(nullable: true));
            AddColumn("dbo.PersonalInfoes", "PassportPage3Id", c => c.Long(nullable: true));
            AddColumn("dbo.PersonalInfoes", "PassportPage4Id", c => c.Long(nullable: true));
            AddColumn("dbo.PersonalInfoes", "PassportPage5Id", c => c.Long(nullable: true));
            CreateIndex("dbo.PersonalInfoes", "PassportPage1Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage2Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage3Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage4Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage5Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage1Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage2Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage3Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage4Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage5Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage5Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage4Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage3Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage2Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage1Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage5Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage4Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage3Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage2Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage1Id" });
            DropColumn("dbo.PersonalInfoes", "PassportPage5Id");
            DropColumn("dbo.PersonalInfoes", "PassportPage4Id");
            DropColumn("dbo.PersonalInfoes", "PassportPage3Id");
            DropColumn("dbo.PersonalInfoes", "PassportPage2Id");
            DropColumn("dbo.PersonalInfoes", "PassportPage1Id");
        }
    }
}
