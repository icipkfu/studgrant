namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationQueues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentId = c.Long(nullable: false),
                        GrantId = c.Long(),
                        Email = c.String(),
                        NotificationType = c.Int(nullable: false),
                        Parameters = c.String(),
                        Sent = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grants", t => t.GrantId)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.GrantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationQueues", "StudentId", "dbo.Students");
            DropForeignKey("dbo.NotificationQueues", "GrantId", "dbo.Grants");
            DropIndex("dbo.NotificationQueues", new[] { "GrantId" });
            DropIndex("dbo.NotificationQueues", new[] { "StudentId" });
            DropTable("dbo.NotificationQueues");
        }
    }
}
