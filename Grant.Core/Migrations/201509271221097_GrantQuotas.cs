namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrantQuotas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrantQuotas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GrantId = c.Long(nullable: false),
                        UniversityId = c.Long(nullable: false),
                        Quota = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grants", t => t.GrantId, cascadeDelete: true)
                .ForeignKey("dbo.Universities", t => t.UniversityId, cascadeDelete: true)
                .Index(t => t.GrantId)
                .Index(t => t.UniversityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantQuotas", "UniversityId", "dbo.Universities");
            DropForeignKey("dbo.GrantQuotas", "GrantId", "dbo.Grants");
            DropIndex("dbo.GrantQuotas", new[] { "UniversityId" });
            DropIndex("dbo.GrantQuotas", new[] { "GrantId" });
            DropTable("dbo.GrantQuotas");
        }
    }
}
