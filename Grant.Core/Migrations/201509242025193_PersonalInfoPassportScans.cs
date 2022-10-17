namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoPassportScans : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonalInfoFiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileHash = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        PersonalInfo_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonalInfoes", t => t.PersonalInfo_Id)
                .Index(t => t.PersonalInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoFiles", "PersonalInfo_Id", "dbo.PersonalInfoes");
            DropIndex("dbo.PersonalInfoFiles", new[] { "PersonalInfo_Id" });
            DropTable("dbo.PersonalInfoFiles");
        }
    }
}
