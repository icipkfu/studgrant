namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantEventFullInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantEvents", "Description", c => c.String());
            AddColumn("dbo.GrantEvents", "Conditions", c => c.String());
            AddColumn("dbo.GrantEvents", "Attachments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantEvents", "Attachments");
            DropColumn("dbo.GrantEvents", "Conditions");
            DropColumn("dbo.GrantEvents", "Description");
        }
    }
}
