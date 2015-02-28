using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Ninject;

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

            var container = new StandardKernel();
            container.Bind<Settings>().To<Settings>().WithConstructorArgument("settings.txt");
            var settings = container.Get<Settings>();
            var rand = new Random(settings.RandomSeed);
            container.Bind<MapGenerator>().To<MapGenerator>()
                .WithConstructorArgument(rand);
            container.Bind<GameVisualizer>().To<GameVisualizer>();
            container.Bind<ProcessMonitor>().To<ProcessMonitor>()
                .WithConstructorArgument(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount))
                .WithConstructorArgument((long)settings.MemoryLimit);
            container.Bind<Ai>().To<Ai>()
                .WithConstructorArgument(aiPath);
            container.Bind<Func<Map, Ai, Game>>().ToMethod(ctx => (map, ai) => new Game(map, ai));
            container.Bind<Func<string, ProcessMonitor, Ai>>().ToMethod(ctx => (exe, monitor) => new Ai(exe, monitor));
            container.Bind<AiTester>().To<AiTester>()
                .WithConstructorArgument(aiPath);
            if (File.Exists(aiPath))
            {
                container.Get<AiTester>().TestSingleFile();
            }
            else
                Console.WriteLine("No AI exe-file " + aiPath);
        }
    }
}