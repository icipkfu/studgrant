namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "PassportState", c => c.Int(nullable: false, defaultValue:0));
            AddColumn("dbo.Students", "StudentBookState", c => c.Int(nullable: false, defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "StudentBookState");
            DropColumn("dbo.Students", "PassportState");
        }
    }
}
