namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class USerEditDateFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "ProfileEditDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Students", "PersonalDataEditDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Students", "RecordBookEditDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "RecordBookEditDate");
            DropColumn("dbo.Students", "PersonalDataEditDate");
            DropColumn("dbo.Students", "ProfileEditDate");
        }
    }
}
