namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigAddFieldBankFilialId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Universities", "BankFilialId", c => c.Long());
            CreateIndex("dbo.Universities", "BankFilialId");
            AddForeignKey("dbo.Universities", "BankFilialId", "dbo.BankFilials", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Universities", "BankFilialId", "dbo.BankFilials");
            DropIndex("dbo.Universities", new[] { "BankFilialId" });
            DropColumn("dbo.Universities", "BankFilialId");
        }
    }
}
