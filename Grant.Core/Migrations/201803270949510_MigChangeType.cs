namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigChangeType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BankFilials", "Code", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BankFilials", "Code", c => c.Int(nullable: false));
        }
    }
}
