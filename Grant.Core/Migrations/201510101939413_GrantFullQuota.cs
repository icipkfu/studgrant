namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantFullQuota : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Grants", "FullQuota", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "FullQuota");
        }
    }
}
