namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigBankFilial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankFilials",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        FilialName = c.String(),
                        Address = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        UserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BankFilials");
        }
    }
}
