namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalWinner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantStudents", "IsAdditionalWinner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStudents", "IsAdditionalWinner");
        }
    }
}
