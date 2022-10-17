namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantAttachments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Grants", "AttachmentFiles", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "AttachmentFiles");
        }
    }
}
