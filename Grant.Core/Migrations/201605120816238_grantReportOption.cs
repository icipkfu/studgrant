namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grantReportOption : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Grants", "CanAddReport", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "CanAddReport");
        }
    }
}
