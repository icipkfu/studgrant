namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "User_Id", "dbo.IdentityUsers");
            DropIndex("dbo.Events", new[] { "User_Id" });
            AddColumn("dbo.Events", "StudentId", c => c.Long(nullable: false));
            CreateIndex("dbo.Events", "StudentId");
            AddForeignKey("dbo.Events", "StudentId", "dbo.Students", "Id", cascadeDelete: true);
            DropColumn("dbo.Events", "EventCreateDate");
            DropColumn("dbo.Events", "UserId");
            DropColumn("dbo.Events", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Events", "UserId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "EventCreateDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Events", "StudentId", "dbo.Students");
            DropIndex("dbo.Events", new[] { "StudentId" });
            DropColumn("dbo.Events", "StudentId");
            CreateIndex("dbo.Events", "User_Id");
            AddForeignKey("dbo.Events", "User_Id", "dbo.IdentityUsers", "Id");
        }
    }
}
