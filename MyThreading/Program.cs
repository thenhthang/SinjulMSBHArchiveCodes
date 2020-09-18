using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyThreading
{
    public class Turn
    {
        public static IList<Turn> _turns = new List<Turn> {
            new Turn{ Title = "Title 1" , IsConnected = false},
            new Turn{ Title = "Title 2" , IsConnected = false },
            new Turn{ Title = "Title 3" , IsConnected = false },
            new Turn{ Title = "Title 4" , IsConnected = false },
            new Turn{ Title = "Title 5" , IsConnected = false },
            new Turn{ Title = "Title 6" , IsConnected = false },
            new Turn{ Title = "Title 7" , IsConnected = false },
            new Turn{ Title = "Title 8" , IsConnected = false },
        };

        public string Title { get; set; }
        public bool IsConnected { get; set; }

        public static bool AnyTurn() =>
            _turns.Any(_ => !_.IsConnected);

        public static void AddTurn()
        {
            var turn = _turns.FirstOrDefault(_ => !_.IsConnected);
            _turns.SingleOrDefault(_=>_.Title == turn.Title).IsConnected = true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TimerCallback callback = new TimerCallback(PrintTime);
            Timer timer = new Timer(
                GetTurn, //GetTurn, //PrintTime, //callback 
                "State from Main Method .. !!!!",
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(4));

            Console.WriteLine("Press any key to terminate application .. !!!!");


            Console.ReadKey();
        }

        public static void PrintTime(object state) =>
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " {0}", state);

        public static void GetTurn(object state)
        {
            lock (Turn._turns)
            {
                if (!Turn.AnyTurn()) return;
                Turn.AddTurn();

                foreach (var item in Turn._turns)
                    Console.WriteLine(item.Title + " : " + item.IsConnected);
            }
        }
    }
}
