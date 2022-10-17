namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoSex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "Sex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "Sex");
        }
    }
}
