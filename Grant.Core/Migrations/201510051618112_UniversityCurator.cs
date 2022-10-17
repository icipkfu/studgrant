namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversityCurator : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UniversityCurators",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UniversityId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Universities", t => t.UniversityId, cascadeDelete: true)
                .Index(t => t.UniversityId)
                .Index(t => t.StudentId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniversityCurators", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.UniversityCurators", "StudentId", "dbo.Students");
            DropIndex("dbo.UniversityCurators", new[] { "StudentId" });
            DropIndex("dbo.UniversityCurators", new[] { "UniversityId" });
            DropTable("dbo.UniversityCurators");
        }
    }
}
