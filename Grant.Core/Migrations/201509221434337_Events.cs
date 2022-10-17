namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Events : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrantEvents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GrantId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        Title = c.String(),
                        Subtitle = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        Image = c.String(),
                        Palette = c.String(),
                        Content = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Grants", t => t.GrantId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityUsers", t => t.User_Id)
                .Index(t => t.GrantId)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantEvents", "User_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.GrantEvents", "GrantId", "dbo.Grants");
            DropIndex("dbo.GrantEvents", new[] { "User_Id" });
            DropIndex("dbo.GrantEvents", new[] { "GrantId" });
            DropTable("dbo.GrantEvents");
        }
    }
}
