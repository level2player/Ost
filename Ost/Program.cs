
using System;
using System.Threading;

namespace Ost
{
    class Program
    {
        static void Main(string[] args)
        {
            var isComplete = false;
            OstServices.StartOst((isok) =>
            {
                Console.WriteLine("ost execute complete");
                isComplete = true;
            });
            while (true)
            {
                if (isComplete)
                {
                    Console.ReadKey();
                    break;
                }
                Thread.Sleep(100);
            }
        }
    }
}
