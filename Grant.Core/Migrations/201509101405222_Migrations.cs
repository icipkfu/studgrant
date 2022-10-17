namespace Grant.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievements",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        Description1 = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        Portfolio_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Portfolios", t => t.Portfolio_Id)
                .Index(t => t.Portfolio_Id);
            
            CreateTable(
                "dbo.CompetitionConditions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataFiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Content = c.Binary(),
                        Extension = c.String(),
                        Size = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        CompetitionCondition_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompetitionConditions", t => t.CompetitionCondition_Id)
                .Index(t => t.CompetitionCondition_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Body = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Event_Id = c.Long(),
                        Grant_Id = c.Long(),
                        Grant_Id1 = c.Long(),
                        Grant_Id2 = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .ForeignKey("dbo.Grants", t => t.Grant_Id)
                .ForeignKey("dbo.Grants", t => t.Grant_Id1)
                .ForeignKey("dbo.Grants", t => t.Grant_Id2)
                .Index(t => t.Event_Id)
                .Index(t => t.Grant_Id)
                .Index(t => t.Grant_Id1)
                .Index(t => t.Grant_Id2);
            
            CreateTable(
                "dbo.GrantFileInfoes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(),
                        Extension = c.String(),
                        Path = c.String(),
                        Size = c.Long(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Grants",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        ExpiresDate = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        CompetitionCondition_Id = c.Long(),
                        Status_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompetitionConditions", t => t.CompetitionCondition_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .Index(t => t.CompetitionCondition_Id)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Portfolios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LastName = c.String(),
                        Patronymic = c.String(),
                        Departament = c.String(),
                        Sex = c.Int(nullable: false),
                        AboutSelf = c.String(),
                        SocialNetworkLinks = c.String(),
                        Role = c.Int(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                        Avatar_Id = c.Long(),
                        Portfolio_Id = c.Long(),
                        University_Id = c.Long(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFiles", t => t.Avatar_Id)
                .ForeignKey("dbo.Portfolios", t => t.Portfolio_Id)
                .ForeignKey("dbo.Universities", t => t.University_Id)
                .ForeignKey("dbo.IdentityUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Avatar_Id)
                .Index(t => t.Portfolio_Id)
                .Index(t => t.University_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Universities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(),
                        Curator = c.String(),
                        Name = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(nullable: false),
                        DeletedMark = c.Boolean(nullable: false),
                    })
                    .Index(t => t.Name, unique: true)
                    .Index(t => t.Curator, unique: true)
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "User_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.Students", "University_Id", "dbo.Universities");
            DropForeignKey("dbo.Students", "Portfolio_Id", "dbo.Portfolios");
            DropForeignKey("dbo.Students", "Avatar_Id", "dbo.DataFiles");
            DropForeignKey("dbo.Achievements", "Portfolio_Id", "dbo.Portfolios");
            DropForeignKey("dbo.Grants", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.IdentityUsers", "Grant_Id2", "dbo.Grants");
            DropForeignKey("dbo.Grants", "CompetitionCondition_Id", "dbo.CompetitionConditions");
            DropForeignKey("dbo.IdentityUsers", "Grant_Id1", "dbo.Grants");
            DropForeignKey("dbo.IdentityUsers", "Grant_Id", "dbo.Grants");
            DropForeignKey("dbo.IdentityUsers", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.DataFiles", "CompetitionCondition_Id", "dbo.CompetitionConditions");
            DropIndex("dbo.Students", new[] { "User_Id" });
            DropIndex("dbo.Students", new[] { "University_Id" });
            DropIndex("dbo.Students", new[] { "Portfolio_Id" });
            DropIndex("dbo.Students", new[] { "Avatar_Id" });
            DropIndex("dbo.Achievements", new[] { "Portfolio_Id" });
            DropIndex("dbo.Grants", new[] { "Status_Id" });
            DropIndex("dbo.IdentityUsers", new[] { "Grant_Id2" });
            DropIndex("dbo.Grants", new[] { "CompetitionCondition_Id" });
            DropIndex("dbo.IdentityUsers", new[] { "Grant_Id1" });
            DropIndex("dbo.IdentityUsers", new[] { "Grant_Id" });
            DropIndex("dbo.IdentityUsers", new[] { "Event_Id" });
            DropIndex("dbo.DataFiles", new[] { "CompetitionCondition_Id" });
            DropTable("dbo.Universities");
            DropTable("dbo.Students");
            DropTable("dbo.Portfolios");
            DropTable("dbo.Status");
            DropTable("dbo.Grants");
            DropTable("dbo.GrantFileInfoes");
            DropTable("dbo.IdentityUsers");
            DropTable("dbo.Events");
            DropTable("dbo.DataFiles");
            DropTable("dbo.CompetitionConditions");
            DropTable("dbo.Achievements");
        }
    }
}
