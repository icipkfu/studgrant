namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Guid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantFileInfoes", "Guid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantFileInfoes", "Guid");
        }
    }
}
