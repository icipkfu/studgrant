namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecordBookFiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "RecordBookFiles", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "RecordBookFiles");
        }
    }
}
