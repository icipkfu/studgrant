namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSocailActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantStudents", "IsSocialActive", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.GrantStudents", "IsSocialHelp", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStudents", "IsSocialHelp");
            DropColumn("dbo.GrantStudents", "IsSocialActive");
        }
    }
}
