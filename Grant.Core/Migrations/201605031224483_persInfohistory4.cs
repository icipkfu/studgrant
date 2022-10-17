namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class persInfohistory4 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.PersonalInfoHistories", "EditTime", c => c.DateTime(nullable: false));
            //AddColumn("dbo.PersonalInfoHistories", "StudentId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonalInfoHistories", "StudentId");
            DropColumn("dbo.PersonalInfoHistories", "EditTime");
        }
    }
}
