namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantEventQuotaChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantEvents", "QuotaChanged", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantEvents", "QuotaChanged");
        }
    }
}
