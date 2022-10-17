namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "User_Id", "dbo.IdentityUsers");
            DropIndex("dbo.Students", new[] { "User_Id" });
            DropColumn("dbo.Students", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Students", "User_Id");
            AddForeignKey("dbo.Students", "User_Id", "dbo.IdentityUsers", "Id", cascadeDelete: true);
        }
    }
}
