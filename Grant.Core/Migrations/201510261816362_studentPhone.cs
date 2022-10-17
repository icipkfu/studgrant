namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentPhone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Phone", c => c.String());
            AddColumn("dbo.Students", "AvatarFileId", c => c.Long());
            CreateIndex("dbo.Students", "AvatarFileId");
            AddForeignKey("dbo.Students", "AvatarFileId", "dbo.GrantFileInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "AvatarFileId", "dbo.GrantFileInfoes");
            DropIndex("dbo.Students", new[] { "AvatarFileId" });
            DropColumn("dbo.Students", "AvatarFileId");
            DropColumn("dbo.Students", "Phone");
        }
    }
}
