namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WinnerReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantQuotas", "WinnerReport", c => c.String());
            AddColumn("dbo.GrantQuotas", "AdditionalWinnerReport", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantQuotas", "AdditionalWinnerReport");
            DropColumn("dbo.GrantQuotas", "WinnerReport");
        }
    }
}
