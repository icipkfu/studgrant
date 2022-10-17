namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "Score", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "Score");
        }
    }
}
