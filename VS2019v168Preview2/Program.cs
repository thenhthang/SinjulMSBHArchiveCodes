using System;

namespace VS2019v168Preview2
{
    class Program
    {
        public class SinjulMSBHBase
        {
            public int Id { get; set; }
        }

        public class SinjulMSBH : SinjulMSBHBase
        {
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            {
                int max = Math.Max(4, 8);

                _ = typeof(Program).Name;
                _ = nameof(Program);
            }
        }
    }
}
