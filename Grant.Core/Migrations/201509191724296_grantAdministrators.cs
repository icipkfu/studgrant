namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grantAdministrators : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IdentityUsers", "Grant_Id1", "dbo.Grants");
            DropIndex("dbo.IdentityUsers", new[] { "Grant_Id2" });
            DropColumn("dbo.IdentityUsers", "Grant_Id1");
            RenameColumn(table: "dbo.IdentityUsers", name: "Grant_Id2", newName: "Grant_Id1");
            AddColumn("dbo.Grants", "Administrators", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grants", "Administrators");
            RenameColumn(table: "dbo.IdentityUsers", name: "Grant_Id1", newName: "Grant_Id2");
            AddColumn("dbo.IdentityUsers", "Grant_Id1", c => c.Long());
            CreateIndex("dbo.IdentityUsers", "Grant_Id2");
            AddForeignKey("dbo.IdentityUsers", "Grant_Id1", "dbo.Grants", "Id");
        }
    }
}
