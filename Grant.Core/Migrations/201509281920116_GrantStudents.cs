namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantStudents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrantStudents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GrantId = c.Long(nullable: false),
                        UniversityId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        IsWinner = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grants", t => t.GrantId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Universities", t => t.UniversityId, cascadeDelete: true)
                .Index(t => t.GrantId)
                .Index(t => t.UniversityId)
                .Index(t => t.StudentId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantStudents", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.GrantStudents", "StudentId", "dbo.Students");
            DropForeignKey("dbo.GrantStudents", "GrantId", "dbo.Grants");
            DropIndex("dbo.GrantStudents", new[] { "StudentId" });
            DropIndex("dbo.GrantStudents", new[] { "UniversityId" });
            DropIndex("dbo.GrantStudents", new[] { "GrantId" });

            DropTable("dbo.GrantStudents");
        }
    }
}
