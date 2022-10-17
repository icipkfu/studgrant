namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Curator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Universities", "ImageFile", c => c.String());
            DropColumn("dbo.Universities", "Curator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Universities", "Curator", c => c.String());
            DropColumn("dbo.Universities", "ImageFile");
        }
    }
}
