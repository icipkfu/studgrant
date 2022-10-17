namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TelegramUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TelegramUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        Username = c.String(),
                        CanSendCommmand = c.Boolean(nullable: false),
                        PictureId = c.Int(),
                        TelegramId = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        UserId = c.Long(),
                        Picture_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantFileInfoes", t => t.Picture_Id)
                .Index(t => t.Picture_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUsers", "Picture_Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.TelegramUsers", new[] { "Picture_Id" });
            DropTable("dbo.TelegramUsers");
        }
    }
}
