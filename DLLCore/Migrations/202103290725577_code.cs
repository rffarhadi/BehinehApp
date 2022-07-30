namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RawDetailes", "DetaileCode", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RawDetailes", "DetaileCode");
        }
    }
}
