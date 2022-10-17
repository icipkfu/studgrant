namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantEvents", "ChangeAdmin", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantEvents", "ChangeAdmin");
        }
    }
}
