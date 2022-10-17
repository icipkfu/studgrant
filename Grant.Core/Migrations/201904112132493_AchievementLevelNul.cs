namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementLevelNul : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Achievements", "Level", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Achievements", "Level", c => c.Int(nullable: false));
        }
    }
}
