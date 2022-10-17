namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Grants", "ImageFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "ImageFile");
        }
    }
}
