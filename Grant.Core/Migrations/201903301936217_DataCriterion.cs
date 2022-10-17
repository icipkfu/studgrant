namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataCriterion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "Criterion", c => c.Int());
            DropColumn("dbo.Achievements", "Сriterion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Achievements", "Сriterion", c => c.Int());
            DropColumn("dbo.Achievements", "Criterion");
        }
    }
}
