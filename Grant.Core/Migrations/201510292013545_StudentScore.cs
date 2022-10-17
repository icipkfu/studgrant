namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Score", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Score");
        }
    }
}
