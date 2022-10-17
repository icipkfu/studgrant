namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversityTown : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Universities", "Town", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Universities", "Town");
        }
    }
}
