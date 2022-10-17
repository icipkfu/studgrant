namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class live : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonalInfoes", "LiveRepublic", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveZone", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveIndex", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveCity", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LivePlace", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveStreet", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveHouse", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveHousing", c => c.String());
            AddColumn("dbo.PersonalInfoes", "LiveFlat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoes", "LiveFlat");
            DropColumn("dbo.PersonalInfoes", "LiveHousing");
            DropColumn("dbo.PersonalInfoes", "LiveHouse");
            DropColumn("dbo.PersonalInfoes", "LiveStreet");
            DropColumn("dbo.PersonalInfoes", "LivePlace");
            DropColumn("dbo.PersonalInfoes", "LiveCity");
            DropColumn("dbo.PersonalInfoes", "LiveIndex");
            DropColumn("dbo.PersonalInfoes", "LiveZone");
            DropColumn("dbo.PersonalInfoes", "LiveRepublic");
        }
    }
}
