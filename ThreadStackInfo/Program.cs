using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadStackInfo
{
    public static class Extensions
    {
        public static void StartAndJoin(this Thread thread, string header)
        {
            thread.Start(header);
            thread.Join();
        }
    }

    class Program
    {
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        static extern void GetCurrentThreadStackLimits(out uint lowLimit, out uint highLimit);

        static void WriteAllocatedStackSize(object header)
        {
            GetCurrentThreadStackLimits(out var low, out var high);
            Console.WriteLine($"{header,-19}:  {((high - low) / 1024),4} KB threadid = {GetCurrentThreadId()}");
        }

        static void Main(string[] args)
        {
            WriteAllocatedStackSize("Main    Stack Size");

            new Thread(WriteAllocatedStackSize, 1024 * 0).StartAndJoin("Default Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 128).StartAndJoin(" 128 KB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 256).StartAndJoin(" 256 KB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 512).StartAndJoin(" 512 KB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 1024).StartAndJoin("   1 MB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 2048).StartAndJoin("   2 MB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 4096).StartAndJoin("   4 MB Stack Size");
            new Thread(WriteAllocatedStackSize, 1024 * 8192).StartAndJoin("   8 MB Stack Size");

            Task.Factory.StartNew(() => WriteAllocatedStackSize("Task Default stack size"));
            Console.ReadKey();
        }
    }
}
