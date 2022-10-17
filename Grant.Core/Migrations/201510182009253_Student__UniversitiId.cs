namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Student__UniversitiId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "UniversityId", "dbo.Universities");
            DropIndex("dbo.Students", new[] { "UniversityId" });
            AlterColumn("dbo.Students", "UniversityId", c => c.Long());
            CreateIndex("dbo.Students", "UniversityId");
            AddForeignKey("dbo.Students", "UniversityId", "dbo.Universities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "UniversityId", "dbo.Universities");
            DropIndex("dbo.Students", new[] { "UniversityId" });
            AlterColumn("dbo.Students", "UniversityId", c => c.Long(nullable: false));
            CreateIndex("dbo.Students", "UniversityId");
            AddForeignKey("dbo.Students", "UniversityId", "dbo.Universities", "Id", cascadeDelete: true);
        }
    }
}
