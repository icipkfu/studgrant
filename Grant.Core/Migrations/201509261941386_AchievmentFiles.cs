namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievmentFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "Files", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "Files");
        }
    }
}
