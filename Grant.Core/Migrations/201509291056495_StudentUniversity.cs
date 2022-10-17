namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentUniversity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "University_Id", "dbo.Universities");
            DropIndex("dbo.Students", new[] { "University_Id" });
            RenameColumn(table: "dbo.Students", name: "University_Id", newName: "UniversityId");
            AlterColumn("dbo.Students", "UniversityId", c => c.Long(nullable: false));
            CreateIndex("dbo.Students", "UniversityId");
            AddForeignKey("dbo.Students", "UniversityId", "dbo.Universities", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "UniversityId", "dbo.Universities");
            DropIndex("dbo.Students", new[] { "UniversityId" });
            AlterColumn("dbo.Students", "UniversityId", c => c.Long());
            RenameColumn(table: "dbo.Students", name: "UniversityId", newName: "University_Id");
            CreateIndex("dbo.Students", "University_Id");
            AddForeignKey("dbo.Students", "University_Id", "dbo.Universities", "Id");
        }
    }
}
