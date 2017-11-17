using JsonFx.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ost.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class HKTradeTT
    {

        [JsonName("stock_code")]
        public string StockCode { get; set; }

        [JsonName("trade_Log")]
        public List<TradeLog> TradeLogs { get; set; }

        public HKTradeTT()
        {
            TradeLogs = new List<TradeLog>();
        }
    }
    public class TradeLog
    {
        [JsonName("ticker_id")]
        public int TickerId { get; set; }
        [JsonName("cancelled")]
        public string Cancelled { get; set; }

        [JsonName("price")]
        public double TradePrice { get; set; }

        [JsonName("aggr_qty")]
        public double AggrQty { get; set; }

        [JsonName("trade_time")]
        public string TradeTime { get; set; }

        [JsonName("trade_type")]
        public int TradeType { get; set; }
    }
}
