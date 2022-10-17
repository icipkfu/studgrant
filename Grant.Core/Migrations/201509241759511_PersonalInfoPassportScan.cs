namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalInfoPassportScan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "PassportScan", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "PassportScan");
        }
    }
}
