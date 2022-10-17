namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ValidationHistories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ModeratorId = c.Long(nullable: false),
                        ValidationUserId = c.Long(nullable: false),
                        State = c.Int(nullable: false),
                        Target = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Students", t => t.ModeratorId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.ValidationUserId, cascadeDelete: true)
                .Index(t => t.ModeratorId)
                .Index(t => t.ValidationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ValidationHistories", "ValidationUserId", "dbo.Students");
            DropForeignKey("dbo.ValidationHistories", "ModeratorId", "dbo.Students");
            DropIndex("dbo.ValidationHistories", new[] { "ValidationUserId" });
            DropIndex("dbo.ValidationHistories", new[] { "ModeratorId" });
            DropTable("dbo.ValidationHistories");
        }
    }
}
