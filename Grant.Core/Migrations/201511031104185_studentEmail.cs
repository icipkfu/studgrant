namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Email", c => c.String());
            AddColumn("dbo.PersonalInfoes", "Agreement", c => c.Boolean(nullable: false));
            AddColumn("dbo.PersonalInfoes", "PassportPage8Id", c => c.Long());
            CreateIndex("dbo.PersonalInfoes", "PassportPage8Id");
            AddForeignKey("dbo.PersonalInfoes", "PassportPage8Id", "dbo.GrantFileInfoes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalInfoes", "PassportPage8Id", "dbo.GrantFileInfoes");
            DropIndex("dbo.PersonalInfoes", new[] { "PassportPage8Id" });
            DropColumn("dbo.PersonalInfoes", "PassportPage8Id");
            DropColumn("dbo.PersonalInfoes", "Agreement");
            DropColumn("dbo.Students", "Email");
        }
    }
}
