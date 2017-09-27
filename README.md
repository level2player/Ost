## Ost
可以读取网易股票A股市场所以的历史行情：
* GetStockList()（爬A股股票代码列表,http://quote.eastmoney.com/stocklist.html#sh）
* LoadCSVData（下载A股股票历史行情,格式csv,http://quotes.money.163.com/service/chddata.html?code={0}）
* GetSimpleStockinfo（读取data路径下面所以csv文件,生成数据集合）
* PostStockInfo（单线程发送历史行情给服务端 http://www.zenyu.site/data/webapinsertstockinfo）
* AsyPostStockInfo（多线程发送历史行情给服务端 http://www.zenyu.site/data/webapinsertstockinfo）

