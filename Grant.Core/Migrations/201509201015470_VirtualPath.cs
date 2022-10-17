namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VirtualPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantFileInfoes", "VirtualPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantFileInfoes", "VirtualPath");
        }
    }
}
