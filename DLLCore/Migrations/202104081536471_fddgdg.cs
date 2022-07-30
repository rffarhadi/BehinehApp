namespace DLLCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fddgdg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecurityHistories", "BourseSymbol", c => c.String());
            AddColumn("dbo.SecurityHistories", "CoID", c => c.Int());
            AddColumn("dbo.SecurityHistories", "PrecedencyRight", c => c.Boolean());
            AddColumn("dbo.SecurityHistories", "TseSymbolCode", c => c.String());
            AddColumn("dbo.SecurityHistories", "FullTitle", c => c.String());
            AddColumn("dbo.SecurityHistories", "TradeDateGre", c => c.DateTime());
            AddColumn("dbo.SecurityHistories", "Roundate", c => c.DateTime());
            AddColumn("dbo.SecurityHistories", "TradeDate", c => c.String());
            AddColumn("dbo.SecurityHistories", "MaxPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "AdjustedMaxPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "minPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "AdjustedMinPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "OpeningPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "AdjustedOpeningPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "ClosingPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "AdjustedClosingPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "AdjustedLastPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "TradeVolume", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "TradeValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "TradeQty", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "PreviousClosingPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "ClosingPriceChange", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "ClosingPChgPercent", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "ShareCount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.SecurityHistories", "MarketValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SecurityHistories", "LastPrice", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.SecurityHistories", "SymbolCoId");
            DropColumn("dbo.SecurityHistories", "Name");
            DropColumn("dbo.SecurityHistories", "SymbolName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecurityHistories", "SymbolName", c => c.String());
            AddColumn("dbo.SecurityHistories", "Name", c => c.String());
            AddColumn("dbo.SecurityHistories", "SymbolCoId", c => c.Int(nullable: false));
            AlterColumn("dbo.SecurityHistories", "LastPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SecurityHistories", "MarketValue");
            DropColumn("dbo.SecurityHistories", "ShareCount");
            DropColumn("dbo.SecurityHistories", "ClosingPChgPercent");
            DropColumn("dbo.SecurityHistories", "ClosingPriceChange");
            DropColumn("dbo.SecurityHistories", "PreviousClosingPrice");
            DropColumn("dbo.SecurityHistories", "TradeQty");
            DropColumn("dbo.SecurityHistories", "TradeValue");
            DropColumn("dbo.SecurityHistories", "TradeVolume");
            DropColumn("dbo.SecurityHistories", "AdjustedLastPrice");
            DropColumn("dbo.SecurityHistories", "AdjustedClosingPrice");
            DropColumn("dbo.SecurityHistories", "ClosingPrice");
            DropColumn("dbo.SecurityHistories", "AdjustedOpeningPrice");
            DropColumn("dbo.SecurityHistories", "OpeningPrice");
            DropColumn("dbo.SecurityHistories", "AdjustedMinPrice");
            DropColumn("dbo.SecurityHistories", "minPrice");
            DropColumn("dbo.SecurityHistories", "AdjustedMaxPrice");
            DropColumn("dbo.SecurityHistories", "MaxPrice");
            DropColumn("dbo.SecurityHistories", "TradeDate");
            DropColumn("dbo.SecurityHistories", "Roundate");
            DropColumn("dbo.SecurityHistories", "TradeDateGre");
            DropColumn("dbo.SecurityHistories", "FullTitle");
            DropColumn("dbo.SecurityHistories", "TseSymbolCode");
            DropColumn("dbo.SecurityHistories", "PrecedencyRight");
            DropColumn("dbo.SecurityHistories", "CoID");
            DropColumn("dbo.SecurityHistories", "BourseSymbol");
        }
    }
}
