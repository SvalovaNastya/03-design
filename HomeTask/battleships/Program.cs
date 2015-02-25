using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace battleships
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: {0} <ai.exe>", Process.GetCurrentProcess().ProcessName);
                return;
            }
            var aiPath = args[0];
            var settings = new Settings("settings.txt");
            var gen = new MapGenerator(settings, new Random(settings.RandomSeed));
            var vis = new GameVisualizer();
            var headersArray = new object[] { "AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Games", "Score" };
            var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);

            if (File.Exists(aiPath))
            {
                var ai = new Ai(aiPath, monitor);
                var tester = new AiTester(settings, gen, vis, monitor, ai, headersArray);
                tester.TestSingleFile(aiPath);
            }
            else
                Console.WriteLine("No AI exe-file " + aiPath);
        }
    }
}