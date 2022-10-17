namespace Grant.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class GrantEvent3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantEvents", "StudentId", c => c.Long(nullable: true));
            AddColumn("dbo.GrantEvents", "EventType", c => c.Int(nullable: false, defaultValue: 0));
            CreateIndex("dbo.GrantEvents", "StudentId");
            AddForeignKey("dbo.GrantEvents", "StudentId", "dbo.Students", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantEvents", "StudentId", "dbo.Students");
            DropIndex("dbo.GrantEvents", new[] { "StudentId" });
            DropColumn("dbo.GrantEvents", "EventType");
            DropColumn("dbo.GrantEvents", "StudentId");
        }
    }
}
