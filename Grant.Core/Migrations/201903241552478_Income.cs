namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Income : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "IncomeFiles", c => c.String());
            AddColumn("dbo.Students", "IncomeState", c => c.Int(nullable: false));
            AddColumn("dbo.Students", "IncomeValidationComment", c => c.String());
            AddColumn("dbo.Students", "IncomeEditDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "IncomeEditDate");
            DropColumn("dbo.Students", "IncomeValidationComment");
            DropColumn("dbo.Students", "IncomeState");
            DropColumn("dbo.Students", "IncomeFiles");
        }
    }
}
