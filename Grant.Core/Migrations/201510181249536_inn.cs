namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "Inn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "Inn");
        }
    }
}
