namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validationHistoryMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ValidationHistories", "ValidationMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ValidationHistories", "ValidationMessage");
        }
    }
}
