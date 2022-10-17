namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementStudentId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Achievements", "Student_Id", "dbo.Students");
            DropIndex("dbo.Achievements", new[] { "Student_Id" });
            RenameColumn(table: "dbo.Achievements", name: "Student_Id", newName: "StudentId");
            AlterColumn("dbo.Achievements", "StudentId", c => c.Long(nullable: false));
            CreateIndex("dbo.Achievements", "StudentId");
            AddForeignKey("dbo.Achievements", "StudentId", "dbo.Students", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Achievements", "StudentId", "dbo.Students");
            DropIndex("dbo.Achievements", new[] { "StudentId" });
            AlterColumn("dbo.Achievements", "StudentId", c => c.Long());
            RenameColumn(table: "dbo.Achievements", name: "StudentId", newName: "Student_Id");
            CreateIndex("dbo.Achievements", "Student_Id");
            AddForeignKey("dbo.Achievements", "Student_Id", "dbo.Students", "Id");
        }
    }
}
