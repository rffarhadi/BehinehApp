namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdf : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RawSubLedgers", "SubLedgerSymbole");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RawSubLedgers", "SubLedgerSymbole", c => c.String());
        }
    }
}
