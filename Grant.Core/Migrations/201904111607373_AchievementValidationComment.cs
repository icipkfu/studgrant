namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementValidationComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "ValidationComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "ValidationComment");
        }
    }
}
