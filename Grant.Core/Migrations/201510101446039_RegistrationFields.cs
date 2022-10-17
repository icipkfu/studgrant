namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistrationFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "RegistrationRepublic", c => c.String());
            AddColumn("dbo.PersonalInfoes", "RegistrationzZone", c => c.String());
            AddColumn("dbo.PersonalInfoes", "RegistrationPlace", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "RegistrationPlace");
            DropColumn("dbo.PersonalInfoes", "RegistrationzZone");
            DropColumn("dbo.PersonalInfoes", "RegistrationRepublic");
        }
    }
}
