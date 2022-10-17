namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IdentityUsers", "Event_Id", "dbo.Events");
            DropIndex("dbo.IdentityUsers", new[] { "Event_Id" });
            AddColumn("dbo.Events", "Subtitle", c => c.String());
            AddColumn("dbo.Events", "Content", c => c.String());
            AddColumn("dbo.Events", "EventCreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "Image", c => c.String());
            AddColumn("dbo.Events", "UserId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "EventType", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Events", "User_Id");
            AddForeignKey("dbo.Events", "User_Id", "dbo.IdentityUsers", "Id");
            DropColumn("dbo.Events", "Body");
            DropColumn("dbo.IdentityUsers", "Event_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IdentityUsers", "Event_Id", c => c.Long());
            AddColumn("dbo.Events", "Body", c => c.String());
            DropForeignKey("dbo.Events", "User_Id", "dbo.IdentityUsers");
            DropIndex("dbo.Events", new[] { "User_Id" });
            DropColumn("dbo.Events", "User_Id");
            DropColumn("dbo.Events", "EventType");
            DropColumn("dbo.Events", "UserId");
            DropColumn("dbo.Events", "Image");
            DropColumn("dbo.Events", "EventCreateDate");
            DropColumn("dbo.Events", "Content");
            DropColumn("dbo.Events", "Subtitle");
            CreateIndex("dbo.IdentityUsers", "Event_Id");
            AddForeignKey("dbo.IdentityUsers", "Event_Id", "dbo.Events", "Id");
        }
    }
}
