using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

internal class Program
{
    //Exam task by Filip Petkov - student number: 2117025
    public static object locker = new object();
    private static void Main(string[] args)
    {
        Random random = new Random();
        Stopwatch stopwatch = new Stopwatch();

        int[] threads = new int[] {1, 1, 2, 5, 10, 50 };

        int N = random.Next();

        int[] arrayOfNumbers = new int[1000000000];
        for (int i = 0; i < arrayOfNumbers.Length; i++)
        {
            arrayOfNumbers[i] = random.Next();
        }
        Console.WriteLine("Array creation Done!");

        foreach (var numOfThreads in threads)
        {
            stopwatch.Reset();
            stopwatch.Start();
            ParallelFindN(arrayOfNumbers, numOfThreads, N);
            stopwatch.Stop();
            Console.WriteLine($"Found a bigger N! Amount of threads used: {numOfThreads} with elapsed time: {stopwatch.Elapsed}.");
        }
    }

    public static void ParallelFindN(int[] array, int numOfThreads, int N)
    {
        bool stopper = false;

        List<Thread> threads = new List<Thread>();
        for (int i = 0; i < numOfThreads; i++)
        {
            Thread t = new Thread(() =>
            {
                int startingIndexForThisThread = (array.Length / numOfThreads) * i;

                FindNFromIndex(array, startingIndexForThisThread, N, stopper);
            });

            t.Start();
            threads.Add(t);
        }

        foreach (var t in threads)
        {
            t.Join();
        }
    }

    public static void FindNFromIndex(int[] array, int index, int N, bool stopper)
    {
        while (stopper == false)
        {
            if (index >= array.Length) break;
            if (array[index] > N)
            {
                lock (locker)
                {
                    if(stopper) break;
                    stopper = true;
                }
            }
            index++;
        }
    }
}