
using System;
using System.Threading;

namespace Ost
{
    class Program
    {
        private static bool IsComplte = true;
        private static string readLineString;
        public static string ReadLineString
        {
            get { return readLineString; }
            set
            {
                if (value == "load"&& IsComplte)
                {
                    readLineString = "load";
                    IsComplte = false;
                    OstServices.StartOstLoadCSV((isok) =>
                    {
                        IsComplte = isok;
                        Console.WriteLine("OstLoadCSV Execute Complete");
                    });
                }
                else if (value == "post"&& IsComplte)
                {
                    IsComplte = false;
                    OstServices.StartOstPost((isok) =>
                    {
                        IsComplte = isok;
                        Console.WriteLine("OstPost Execute Complete");
                    });
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("input load/post~");
            while (true)
            {
                ReadLineString = Console.ReadLine();
                Thread.Sleep(100);
            }
        }
    }
}
