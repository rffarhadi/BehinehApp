using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.DBContext.Entities.StockHistoy
{
    public class SecurityHistory
    {
        [Key]
        public int Id { get; set; }
        public string BourseSymbol { get; set; }
        public Nullable<int> CoID { get; set; }
        public long? TafsilCodeInCds { get; set; }
        public Nullable<bool> PrecedencyRight { get; set; }
        public string TseSymbolCode { get; set; }
        public string FullTitle { get; set; }
        public Nullable<System.DateTime> TradeDateGre { get; set; }
        public Nullable<System.DateTime> Roundate { get; set; }
        public string TradeDate { get; set; }
        public Nullable<decimal> MaxPrice { get; set; }
        public Nullable<decimal> AdjustedMaxPrice { get; set; }
        public Nullable<decimal> minPrice { get; set; }
        public Nullable<decimal> AdjustedMinPrice { get; set; }
        public Nullable<decimal> OpeningPrice { get; set; }
        public Nullable<decimal> AdjustedOpeningPrice { get; set; }
        public Nullable<decimal> ClosingPrice { get; set; }
        public Nullable<decimal> AdjustedClosingPrice { get; set; }
        public Nullable<decimal> LastPrice { get; set; }
        public Nullable<decimal> AdjustedLastPrice { get; set; }
        public Nullable<decimal> TradeVolume { get; set; }
        public Nullable<decimal> TradeValue { get; set; }
        public Nullable<decimal> TradeQty { get; set; }
        public Nullable<decimal> PreviousClosingPrice { get; set; }
        public Nullable<decimal> ClosingPriceChange { get; set; }
        public Nullable<decimal> ClosingPChgPercent { get; set; }
        public Nullable<decimal> ShareCount { get; set; }
        public Nullable<decimal> MarketValue { get; set; }

    }

    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string TokenInDb { get; set; }
        public DateTime Date { get; set; }
    }

    public class APCOtoken
    {
        public string token { get; set; }

    }
}
