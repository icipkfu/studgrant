namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantStatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Grants", "Status_Id", "dbo.Status");
            DropIndex("dbo.Grants", new[] { "Status_Id" });
            AddColumn("dbo.Grants", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.Grants", "Status_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Grants", "Status_Id", c => c.Long());
            DropColumn("dbo.Grants", "Status");
            CreateIndex("dbo.Grants", "Status_Id");
            AddForeignKey("dbo.Grants", "Status_Id", "dbo.Status", "Id");
        }
    }
}
