namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdgdg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecurityHistories", "SymbolCoId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecurityHistories", "SymbolCoId");
        }
    }
}
