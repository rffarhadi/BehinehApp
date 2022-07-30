namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecurityHistories", "TafsilCodeInCds", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecurityHistories", "TafsilCodeInCds");
        }
    }
}
