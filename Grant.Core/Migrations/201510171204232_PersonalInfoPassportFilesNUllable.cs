namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoPassportFilesNUllable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage1Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage2Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage3Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage4Id", "dbo.GrantFileInfoes");
            DropForeignKey("dbo.PersonalInfoes", "PassportPage5Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage1Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage2Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage3Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage4Id" });
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage5Id" });
            AlterColumn("dbo.PersonalInfoes", "PassportPage1Id", c => c.Long());
            AlterColumn("dbo.PersonalInfoes", "PassportPage2Id", c => c.Long());
            AlterColumn("dbo.PersonalInfoes", "PassportPage3Id", c => c.Long());
            AlterColumn("dbo.PersonalInfoes", "PassportPage4Id", c => c.Long());
            AlterColumn("dbo.PersonalInfoes", "PassportPage5Id", c => c.Long());
            CreateIndex("dbo.PersonalInfoes", "PassportPage1Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage2Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage3Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage4Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage5Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage1Id", "dbo.GrantFileInfoes", "Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage2Id", "dbo.GrantFileInfoes", "Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage3Id", "dbo.GrantFileInfoes", "Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage4Id", "dbo.GrantFileInfoes", "Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage5Id", "dbo.GrantFileInfoes", "Id");
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
            AlterColumn("dbo.PersonalInfoes", "PassportPage5Id", c => c.Long(nullable: false));
            AlterColumn("dbo.PersonalInfoes", "PassportPage4Id", c => c.Long(nullable: false));
            AlterColumn("dbo.PersonalInfoes", "PassportPage3Id", c => c.Long(nullable: false));
            AlterColumn("dbo.PersonalInfoes", "PassportPage2Id", c => c.Long(nullable: false));
            AlterColumn("dbo.PersonalInfoes", "PassportPage1Id", c => c.Long(nullable: false));
            CreateIndex("dbo.PersonalInfoes", "PassportPage5Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage4Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage3Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage2Id");
            CreateIndex("dbo.PersonalInfoes", "PassportPage1Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage5Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage4Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage3Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage2Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonalInfoes", "PassportPage1Id", "dbo.GrantFileInfoes", "Id", cascadeDelete: true);
        }
    }
}
