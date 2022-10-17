namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantAdmin2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrantAdmins",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentId = c.Long(nullable: false),
                        GrantId = c.Long(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grants", t => t.GrantId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.GrantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantAdmins", "StudentId", "dbo.Students");
            DropForeignKey("dbo.GrantAdmins", "GrantId", "dbo.Grants");
            DropIndex("dbo.GrantAdmins", new[] { "GrantId" });
            DropIndex("dbo.GrantAdmins", new[] { "StudentId" });
            DropTable("dbo.GrantAdmins");
        }
    }
}
