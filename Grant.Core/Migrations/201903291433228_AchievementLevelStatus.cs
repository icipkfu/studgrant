namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementLevelStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Achievements", "State", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Achievements", "State", c => c.Int(nullable: false));
        }
    }
}
