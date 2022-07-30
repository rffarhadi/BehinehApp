namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _long : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RawSubLedgers", "SubLedgerCode", c => c.Long(nullable: false));
            AlterColumn("dbo.RawLedgers", "RawLedgerCode", c => c.Long(nullable: false));
            AlterColumn("dbo.RawSubCategories", "SubCategoryCode", c => c.Long(nullable: false));
            AlterColumn("dbo.RawCategories", "RawCategoryCode", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RawCategories", "RawCategoryCode", c => c.String());
            AlterColumn("dbo.RawSubCategories", "SubCategoryCode", c => c.String());
            AlterColumn("dbo.RawLedgers", "RawLedgerCode", c => c.String());
            AlterColumn("dbo.RawSubLedgers", "SubLedgerCode", c => c.String());
        }
    }
}
