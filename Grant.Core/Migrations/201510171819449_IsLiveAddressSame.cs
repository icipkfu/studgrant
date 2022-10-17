namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsLiveAddressSame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "IsLiveAddressSame", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "IsLiveAddressSame");
        }
    }
}
