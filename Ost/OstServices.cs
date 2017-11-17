using EasyHttp.Http;
using Ost.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ost
{
    public static class OstServices
    {
        static string URL = "http://www.zenyu.site/data/webapinsertstockinfo";
        static string URL2 = "http://www.zenyu.site/data/webapihktradett";
        static string URLSH = "http://quote.eastmoney.com/stocklist.html#sh";
        static string CsvUrl = "http://quotes.money.163.com/service/chddata.html?code={0}";

        public static async void StartOstLoadCSV(Action<bool> ComplteCallBack)
        {
            var dicStockList = await GetStockList();
            var csvStockList = await LoadCSVData(dicStockList);
            ComplteCallBack(true);
        }

        public static async void StartOstPost(Action<bool> ComplteCallBack)
        {
            //var simpleStockinfoList = await GetSimpleStockInfo();
            // var scount = await PostStockInfo(simpleStockinfoList, URL);
            // AsyPostStockInfo(simpleStockinfoList, (Iscomplte) => { ComplteCallBack(Iscomplte); });
            var hkTradeLog = await GetHkTradeLog();
            var scount = await PostStockInfo(hkTradeLog, URL2);
            ComplteCallBack(true);
           
        }

        public async static Task<Dictionary<string, string>> GetStockList()
        {
            return await Task.Run(() =>
            {
                var dic = new Dictionary<string, string>();
                var http = new HttpClient();
                var resSH = http.Get(URLSH);
                if (resSH.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var htmlStr = resSH.RawText;
                    MatchCollection mlCollection = Regex.Matches(htmlStr, "<li>.*?</li>");
                    foreach (Match ml in mlCollection)
                    {
                        var listr = OstHelper.GetSubstring(ml.Value, "(", ")");
                        if (!string.IsNullOrEmpty(listr) && listr.Length <= 7)
                        {
                            dic.Add(listr, "CN");
                        }
                    }
                }
                return dic;
            });
        }

        public async static Task<List<string>> LoadCSVData(Dictionary<string, string> StockList)
        {
            return await Task.Run(() =>
            {
                var stk_list = new List<string>();
                foreach (var item in StockList)
                {
                    var httpUrl = string.Format(CsvUrl, GetRealStockCode(item.Key));
                    var http = new HttpClient();
                    if (File.Exists($"data/{item.Key}.csv"))
                        File.Delete($"data/{item.Key}.csv");
                    var res = http.GetAsFile(httpUrl, $"data/{item.Key}.csv");
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        stk_list.Add(item.Key);
                        Console.WriteLine($"add {item.Key}.csv,complete.........");
                    }
                }
                return stk_list;
            });
        }

        public async static Task<List<SimpleStockinfo>> GetSimpleStockInfo()
        {
            return await Task.Run(() =>
           {
               var stkList = new List<string>();
               foreach (var item in new DirectoryInfo("data").GetFiles())
                   stkList.Add($"data/{item}");
               var simpleStockinfoList = new List<SimpleStockinfo>();
               stkList.ForEach((csvfile) =>
               {
                   if (File.Exists(csvfile))
                   {
                       var simpleStockinfo = new SimpleStockinfo();
                       var dt = OstHelper.ConvertCSVtoDataTable(csvfile, ',');
                       int index = 0;
                       foreach (DataRow dr in dt.Rows)
                       {
                           index++;
                           if (index == 1)
                           {
                               simpleStockinfo.StockCode = dr["股票代码"].ToString().Replace("'", String.Empty);
                               simpleStockinfo.StockName = dr["名称"].ToString();
                               simpleStockinfo.ExchangeType = "CN";
                           }
                           object value = dr["日期"];
                           var hsData = new HisData();
                           hsData.TradeDate = int.Parse(dr["日期"].ToString().Replace("-", string.Empty));
                           hsData.ClosePrice = GetRealDoubleValue(dr["收盘价"].ToString());
                           hsData.HighPrice = GetRealDoubleValue(dr["最高价"].ToString());
                           hsData.LowPrice = GetRealDoubleValue(dr["最低价"].ToString());
                           hsData.OpenPrice = GetRealDoubleValue(dr["开盘价"].ToString());
                           hsData.PreClolsePrice = GetRealDoubleValue(dr["前收盘"].ToString());
                           hsData.UpdownOver = GetRealDoubleValue(dr["涨跌额"].ToString());
                           hsData.UpdownRange = GetRealDoubleValue(dr["涨跌幅"].ToString());
                           hsData.TurnoveRate = GetRealDoubleValue(dr["换手率"].ToString());
                           hsData.TradeVolume = GetRealDoubleValue(dr["成交量"].ToString());
                           hsData.Turnover = GetRealDoubleValue(dr["成交金额"].ToString());
                           hsData.MarketValue = GetRealDoubleValue(dr["总市值"].ToString());
                           hsData.CirculateMarketValue = GetRealDoubleValue(dr["流通市值"].ToString());
                           simpleStockinfo.HisDataList.Add(hsData);
                       }
                       simpleStockinfoList.Add(simpleStockinfo);
                       Console.WriteLine($"read {csvfile} ok!");
                   }
               });
               simpleStockinfoList.RemoveAll((stock) => { return string.IsNullOrWhiteSpace(stock.StockCode); });
               return simpleStockinfoList;
           });
        }
        /// <summary>
        /// GetHkTradeLog
        /// </summary>
        /// <returns></returns>
        public async static Task<List<HKTradeTT>> GetHkTradeLog()
        {
            return await Task.Run(() =>
            {
                List<HKTradeTT> List = new List<HKTradeTT>();
                DirectoryInfo TheFolder = new DirectoryInfo(@"tradelog");
                foreach (FileInfo fileinfo in TheFolder.GetFiles())
                {
                    var dt = OstHelper.ConvertCSVtoDataTable("tradelog" + "/" + fileinfo.Name, '\t');
                    var hKTradeTT = new HKTradeTT();
                    int index = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        index++;
                        if (index == 1)
                        {
                            hKTradeTT.StockCode = dr["stkcode"].ToString().Replace("'", String.Empty);
                        }
                        var tradeLog = new TradeLog();
                        tradeLog.TickerId = int.Parse(dr["ticker_id"].ToString().Replace("-", string.Empty));
                        tradeLog.Cancelled = dr["cancelled"].ToString();
                        tradeLog.TradePrice = GetRealDoubleValue(dr["price"].ToString());
                        tradeLog.AggrQty = GetRealDoubleValue(dr["aggr_qty"].ToString());
                        tradeLog.TradeTime = (dr["trade_time"].ToString()).Length == 5 ? dr["trade_time"].ToString().Insert(0, "0") : dr["trade_time"].ToString();
                        tradeLog.TradeType = int.Parse(dr["trade_type"].ToString().Replace("-", string.Empty));
                        hKTradeTT.TradeLogs.Add(tradeLog);
                    }
                    List.Add(hKTradeTT);
                }
                return List;
            });
        }

        public async static Task<int> PostStockInfo<T>(List<T> ListData, string Url)
        {
            return await Task.Run(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                int scount = 0;
                var http = new HttpClient();
                foreach (var simpleStockinfo in ListData)
                {
                    Stopwatch watch2 = new Stopwatch();
                    watch2.Start();
                    var response = http.Post(Url, simpleStockinfo, HttpContentTypes.ApplicationJson);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        watch2.Stop();
                        Console.WriteLine($"Post completion,HttpErrorCode={response.StatusCode},Spend time={watch2.ElapsedMilliseconds.ToString("N0")}ms");
                        scount++;
                    }
                    else
                    {
                        watch2.Stop();
                        Console.WriteLine($"Post completion, HttpErrorCode={response.StatusCode},Spend time={watch2.ElapsedMilliseconds.ToString("N0")}ms");
                    }
                }
                watch.Stop();
                Console.WriteLine($"Market data import completion,importcount={scount},Spend time={watch.ElapsedMilliseconds.ToString("N0")}ms");
                return scount;
            });
        }
        public static void AsyPostStockInfo(List<SimpleStockinfo> ListData, Action<bool> CompletedCallBack)
        {
            int count = 0;
            var watch2 = new Stopwatch();
            watch2.Start();
            var http = new HttpClient();
            foreach (var simpleStockinfo in ListData)
            {
                Task.Factory.StartNew(() =>
                {
                    return http.Post(URL, simpleStockinfo, HttpContentTypes.ApplicationJson);
                }).ContinueWith((t) =>
                {
                    if (t.IsCompleted)
                    {
                        count++;
                        Console.WriteLine($"Post StockCode={simpleStockinfo.StockCode},HttpCode={t.Result.StatusCode},Count={count}");
                        if (count == ListData.Count())
                        {
                            watch2.Stop();
                            Console.WriteLine($"Market data import completion,importcount={count},Spend time={watch2.ElapsedMilliseconds.ToString("N0")}ms");
                            CompletedCallBack(true);
                        }
                    }
                });
            }
        }
        private static string GetRealStockCode(string key)
        {
            var tmp = key[0];
            if (tmp == '0' || tmp == '2' || tmp == '3')
                return key.Insert(0, "1");
            else
                return key.Insert(0, "0");
        }

        private static double GetRealDoubleValue(string OldValue)
        {
            double reulst = 0;
            double.TryParse(OldValue, out reulst);
            return reulst;
        }
    }
}
