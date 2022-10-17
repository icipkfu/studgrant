namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversityCurator1 : DbMigration
    {
        public override void Up()
        {   
            AddColumn("dbo.Universities", "CuratorId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Universities", "CuratorId");
        }
    }
}
