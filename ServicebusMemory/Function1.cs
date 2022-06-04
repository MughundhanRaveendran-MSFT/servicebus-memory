using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServicebusMemory
{
    public static class Function1
    {
        private const int MByteSize = 1_048_576;
        [FunctionName("Function1")]
        public static async Task Run([ServiceBusTrigger("salesmessages", Connection = "connection_string")]string myQueueItem, ILogger log)
        {

            log.LogInformation("> Started Run: {StartTime}", DateTime.UtcNow.ToLongTimeString());
            LogMemory(log);

            var buffer = new byte[20][];

            for (var i = 0; i < 20; i++)
            {
                log.LogInformation(" > Allocating 100 MB");
                buffer[i] = new byte[MByteSize * 100];

                LogMemory(log);

                log.LogInformation(" > Filling 100 MB");
                Array.Fill<byte>(buffer[i], 0xFF);

                LogMemory(log);

                await Task.Delay(1000);
            }

            log.LogInformation("> Finished Run: {ExitTime}", DateTime.UtcNow.ToLongTimeString());
            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }

        private static void LogMemory(ILogger logger)
        {
            var proc = Process.GetCurrentProcess();
            logger.LogInformation(" -- WorkingSet64: {WorkingSet64}", ToMb(proc.WorkingSet64));
            logger.LogInformation(" -- VirtualMemorySize64: {VirtualMemorySize64}", ToMb(proc.VirtualMemorySize64));
            logger.LogInformation(" -- PagedMemorySize64: {PagedMemorySize64}", ToMb(proc.PagedMemorySize64));
            logger.LogInformation(" ## GetTotalAllocatedBytes: {GetTotalAllocatedBytes}", ToMb(GC.GetTotalAllocatedBytes()));
        }

        private static long ToMb(long b) => b / MByteSize;
    }
}
