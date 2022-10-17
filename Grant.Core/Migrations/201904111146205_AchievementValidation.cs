namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementValidation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "ValidationState", c => c.Int(nullable: false, defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "ValidationState");
        }
    }
}
