using System.Threading;

namespace AopLoggerTest
{
    internal class Program
    {
        [PerformanceLogContainer]
        public static void Main(string[] args)
        {
            B();
            Thread.Sleep(1000);
        }

        [PerformanceLogRecord]
        public static void B()
        {
            A();
            Thread.Sleep(1000);
        }

        [PerformanceLogRecord]
        public static void A()
        {
            Thread.Sleep(500);
        }
    }
}