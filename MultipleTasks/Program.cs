using System;
using System.Threading.Tasks;

namespace MultipleTasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World .. !!!!");

            Task<int> task1 = Task.Run(() => 13);
            Task<string> task2 = Task.Run(() => "SinjulMSBH");
            Task<string> task3 = Task.Run(() => "JackSlater");

            await Task.WhenAll(task1, task2);
            var task1Result = task1.Result; // or await task1
            var task2Result = task2.GetAwaiter().GetResult(); // or await task2

            //? This doesn't work
            //var (task1Result, task2Result) = await Task.WhenAll(task1, task2);


            //! This works :)
            var (t1Result, t2Result, t3Result) =
                await TaskExtension.WhenAll(task1, task2, task3);
        }
    }
}
