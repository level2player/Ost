using JsonFx.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ost.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class SimpleStockinfo
    {

        [JsonName("stock_code")]
        public string StockCode { get; set; }
        [JsonName("exchange_type")]
        public string ExchangeType { get; set; }
        [JsonName("stock_name")]
        public string StockName { get; set; }

        [JsonName("his_data")]
        public List<HisData> HisDataList { get; set; }

        public SimpleStockinfo()
        {
            HisDataList = new List<HisData>();
        }
    }
    public class HisData
    {
        [JsonName("trade_date")]
        public int TradeDate { get; set; }
        [JsonName("close_price")]
        public double ClosePrice { get; set; }

        [JsonName("high_price")]
        public double HighPrice { get; set; }

        [JsonName("low_price")]
        public double LowPrice { get; set; }

        [JsonName("open_price")]
        public double OpenPrice { get; set; }

        [JsonName("preClolse_price")]
        public double PreClolsePrice { get; set; }

        [JsonName("updown_over")]
        public double UpdownOver { get; set; }

        [JsonName("updown_range")]
        public double UpdownRange { get; set; }

        [JsonName("turnove_rate")]
        public double TurnoveRate { get; set; }
        [JsonName("trade_volume")]
        public double TradeVolume { get; set; }
        [JsonName("turnover")]
        public double Turnover { get; set; }
        [JsonName("market_value")]
        public double MarketValue { get; set; }

        [JsonName("circulate_marketValue")]
        public double CirculateMarketValue { get; set; }
    }
}
