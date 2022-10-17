namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantEventDateChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantEvents", "DateChange", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantEvents", "DateChange");
        }
    }
}
