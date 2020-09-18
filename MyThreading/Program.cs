using System;
using System.Threading;

namespace MyThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            TimerCallback callback = new TimerCallback(PrintTime);
            Timer timer = new Timer(
                PrintTime, //callback 
                "State from Main Method .. !!!!",
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(4));

            Console.WriteLine("Press any key to terminate application .. !!!!");
            Console.ReadKey();
        }

        public static void PrintTime(object state) =>
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " {0}", state);
    }
}
