namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationMsg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "PassValidationComment", c => c.String());
            AddColumn("dbo.Students", "BookValidationComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "BookValidationComment");
            DropColumn("dbo.Students", "PassValidationComment");
        }
    }
}
