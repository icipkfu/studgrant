namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementProof : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievements", "ProofFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievements", "ProofFile");
        }
    }
}
