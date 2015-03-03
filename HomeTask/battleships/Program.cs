using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using NLog;

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
            var tester = new AiTester(settings);
            Logger resultsLog = LogManager.GetLogger("results");
            tester.writeLogInfo += resultsLog.Info;
            if (File.Exists(aiPath))
            {
                var gen = new MapGenerator(settings, new Random(settings.RandomSeed));
                var gamesArray = new List<Game>();
                var ai = new Ai(aiPath);
                for (var gameIndex = 0; gameIndex < settings.GamesCount; gameIndex++)
                {
                    var map = gen.GenerateMap();
                    var game = new Game(map, ai);
                    gamesArray.Add(game);
                }
                tester.TestSingleFile(ai, gamesArray);
            }
            else
                Console.WriteLine("No AI exe-file " + aiPath);
        }
    }
}