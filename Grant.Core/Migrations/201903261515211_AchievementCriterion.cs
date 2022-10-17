namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementCriterion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "Сriterion", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "Сriterion");
        }
    }
}
