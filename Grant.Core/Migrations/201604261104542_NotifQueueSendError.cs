namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotifQueueSendError : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationQueues", "SendError", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotificationQueues", "SendError");
        }
    }
}
