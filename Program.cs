using System;
using System.Threading.Tasks;

namespace Target_Receiver
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Radar Target Collection Process Start {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            if (args.Length < 1)
            {
                Console.WriteLine("[0] : Port");
                return;
            }

            if (!int.TryParse(args[0], out ConfigFile.Port))
            {
                Console.WriteLine("Invalid port number!");
                return;
            }

            ConfigFile.KAFKA = Environment.GetEnvironmentVariable("KAFKA", EnvironmentVariableTarget.Process);

            Console.WriteLine($"KAFKA Server : {ConfigFile.KAFKA}, Client Port : {ConfigFile.Port}");

            TargetClient client = new TargetClient();
            await client.Start();

        }
    }
}
