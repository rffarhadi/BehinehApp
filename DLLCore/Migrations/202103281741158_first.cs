namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InfrencesConclutionProfiles",
                c => new
                    {
                        InfrencesConclutionID = c.Long(nullable: false, identity: true),
                        Inference = c.String(),
                        FinalConclution = c.String(),
                        InvestorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.InfrencesConclutionID)
                .ForeignKey("dbo.InvestorProfiles", t => t.InvestorID, cascadeDelete: true)
                .Index(t => t.InvestorID);
            
            CreateTable(
                "dbo.InvestorProfiles",
                c => new
                    {
                        InvestorID = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        FatherName = c.String(),
                        RegisterDate = c.String(),
                        BirthDate = c.String(),
                        NationalCode = c.String(),
                        BourseCode = c.String(),
                        InvestorCodeInCDS = c.Long(),
                        BirthCertificateID = c.String(),
                        Email = c.String(),
                        Age = c.Int(nullable: false),
                        MobileNumber = c.String(),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        PostalCode = c.String(),
                        InitialEquity = c.Long(nullable: false),
                        ExperiencByMonth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InvestorID);
            
            CreateTable(
                "dbo.InvestorOperations",
                c => new
                    {
                        OprID = c.Long(nullable: false, identity: true),
                        OprExplain = c.String(),
                        OprDate = c.DateTime(nullable: false),
                        MoneyEntry = c.Decimal(precision: 18, scale: 2),
                        MoneyExit = c.Decimal(precision: 18, scale: 2),
                        InvestorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.OprID)
                .ForeignKey("dbo.InvestorProfiles", t => t.InvestorID, cascadeDelete: true)
                .Index(t => t.InvestorID);
            
            CreateTable(
                "dbo.InvestorOSIProfiles",
                c => new
                    {
                        OsiID = c.Long(nullable: false, identity: true),
                        Objective = c.String(),
                        Subjective = c.String(),
                        InvestorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.OsiID)
                .ForeignKey("dbo.InvestorProfiles", t => t.InvestorID, cascadeDelete: true)
                .Index(t => t.InvestorID);
            
            CreateTable(
                "dbo.Journals",
                c => new
                    {
                        EntryID = c.Long(nullable: false, identity: true),
                        EntryNo = c.Long(nullable: false),
                        EntryGregorianDate = c.DateTime(nullable: false),
                        EntryDate = c.String(nullable: false),
                        EntryDescription = c.String(),
                        EntryDebit = c.Long(nullable: false),
                        EntryCredit = c.Long(nullable: false),
                        EntryBuySellVolume = c.Long(),
                        TradePrice = c.Long(),
                        TradeType = c.String(),
                        FactorNumber = c.Long(),
                        BrokerFee = c.Long(),
                        TotalFee = c.Long(),
                        TradeTax = c.Long(),
                        BrokerName = c.String(),
                        BrokerCode = c.Int(),
                        DetaileID = c.Long(nullable: false),
                        InvestorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.EntryID)
                .ForeignKey("dbo.InvestorProfiles", t => t.InvestorID, cascadeDelete: true)
                .ForeignKey("dbo.RawDetailes", t => t.DetaileID, cascadeDelete: true)
                .Index(t => t.DetaileID)
                .Index(t => t.InvestorID);
            
            CreateTable(
                "dbo.RawDetailes",
                c => new
                    {
                        DetaileID = c.Long(nullable: false, identity: true),
                        DetaileName = c.String(nullable: false),
                        DetaileSymbole = c.String(),
                        TafsilCodeInCDS = c.Long(),
                        IsPrecedence = c.Boolean(nullable: false),
                        IsMarketMakerContrary = c.Boolean(nullable: false),
                        SubLedgerID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.DetaileID)
                .ForeignKey("dbo.RawSubLedgers", t => t.SubLedgerID, cascadeDelete: true)
                .Index(t => t.SubLedgerID);
            
            CreateTable(
                "dbo.RawSubLedgers",
                c => new
                    {
                        SubLedgerID = c.Long(nullable: false, identity: true),
                        SubLedgerCode = c.String(),
                        SubLedgerName = c.String(),
                        SubLedgerSymbole = c.String(),
                        RawLedgerID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.SubLedgerID)
                .ForeignKey("dbo.RawLedgers", t => t.RawLedgerID, cascadeDelete: true)
                .Index(t => t.RawLedgerID);
            
            CreateTable(
                "dbo.RawLedgers",
                c => new
                    {
                        RawLedgerID = c.Long(nullable: false, identity: true),
                        RawLedgerCode = c.String(),
                        RawLedgerName = c.String(),
                        SubCategoryID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.RawLedgerID)
                .ForeignKey("dbo.RawSubCategories", t => t.SubCategoryID, cascadeDelete: true)
                .Index(t => t.SubCategoryID);
            
            CreateTable(
                "dbo.RawSubCategories",
                c => new
                    {
                        SubCategoryID = c.Long(nullable: false, identity: true),
                        SubCategoryCode = c.String(),
                        SubCategoryName = c.String(),
                        RawCategoryID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.SubCategoryID)
                .ForeignKey("dbo.RawCategories", t => t.RawCategoryID, cascadeDelete: true)
                .Index(t => t.RawCategoryID);
            
            CreateTable(
                "dbo.RawCategories",
                c => new
                    {
                        RawCategoryID = c.Long(nullable: false, identity: true),
                        RawCategoryCode = c.String(),
                        RawCategoryName = c.String(),
                    })
                .PrimaryKey(t => t.RawCategoryID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RawSubLedgers", "RawLedgerID", "dbo.RawLedgers");
            DropForeignKey("dbo.RawLedgers", "SubCategoryID", "dbo.RawSubCategories");
            DropForeignKey("dbo.RawSubCategories", "RawCategoryID", "dbo.RawCategories");
            DropForeignKey("dbo.RawDetailes", "SubLedgerID", "dbo.RawSubLedgers");
            DropForeignKey("dbo.Journals", "DetaileID", "dbo.RawDetailes");
            DropForeignKey("dbo.Journals", "InvestorID", "dbo.InvestorProfiles");
            DropForeignKey("dbo.InvestorOSIProfiles", "InvestorID", "dbo.InvestorProfiles");
            DropForeignKey("dbo.InvestorOperations", "InvestorID", "dbo.InvestorProfiles");
            DropForeignKey("dbo.InfrencesConclutionProfiles", "InvestorID", "dbo.InvestorProfiles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RawSubCategories", new[] { "RawCategoryID" });
            DropIndex("dbo.RawLedgers", new[] { "SubCategoryID" });
            DropIndex("dbo.RawSubLedgers", new[] { "RawLedgerID" });
            DropIndex("dbo.RawDetailes", new[] { "SubLedgerID" });
            DropIndex("dbo.Journals", new[] { "InvestorID" });
            DropIndex("dbo.Journals", new[] { "DetaileID" });
            DropIndex("dbo.InvestorOSIProfiles", new[] { "InvestorID" });
            DropIndex("dbo.InvestorOperations", new[] { "InvestorID" });
            DropIndex("dbo.InfrencesConclutionProfiles", new[] { "InvestorID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RawCategories");
            DropTable("dbo.RawSubCategories");
            DropTable("dbo.RawLedgers");
            DropTable("dbo.RawSubLedgers");
            DropTable("dbo.RawDetailes");
            DropTable("dbo.Journals");
            DropTable("dbo.InvestorOSIProfiles");
            DropTable("dbo.InvestorOperations");
            DropTable("dbo.InvestorProfiles");
            DropTable("dbo.InfrencesConclutionProfiles");
        }
    }
}
