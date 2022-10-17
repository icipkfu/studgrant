namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Grants", "Conditions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "Conditions");
        }
    }
}
