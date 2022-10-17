namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "ImageFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "ImageFile");
        }
    }
}
