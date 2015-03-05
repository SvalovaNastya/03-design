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
            if (File.Exists(aiPath))
            {
                RunGames(aiPath);
            }
            else
                Console.WriteLine("No AI exe-file " + aiPath);
        }

        private static void RunGames(string aiPath)
        {
            var settings = new Settings("settings.txt");
            var tester = new GameRunner(settings);
            Logger resultsLog = LogManager.GetLogger("results");
            var gen = new MapGenerator(settings, new Random(settings.RandomSeed));
//            var ai = new Ai(aiPath);
            var vis = new GameVisualizer();
            var stat = new Statistic();
            var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);
            var aiPool = new AiPool(settings.CrashLimit, monitor, aiPath);
            var ai = aiPool.GetNewAi();
//            ai.registerProcess += monitor.Register;
            var cVis = new ConsVisualiser();

            for (var gameIndex = 0; gameIndex < settings.GamesCount; gameIndex++)
            {
                var map = gen.GenerateMap();
                var game = new Game(map, ai);
                var gameResult = tester.GameRun(game, vis.Visualize);
                stat.crashes += gameResult.crashes;
                if (gameResult.crashes > 0)
                    ai = aiPool.GetNewAi();
//                if (stat.crashes > settings.CrashLimit)
//                    return new GameResult(game.BadShots, 0, crashes);
                stat.badShots += gameResult.badShots;
                if (gameResult.shots != 0)
                {
                    stat.shots.Add(gameResult.shots);

                    if (settings.Verbose)
                    {
                        cVis.WriteGameStat(game, gameIndex);
                    }
                }
                stat.gamesPlayed++;
            }

            ai.Dispose();
            var message = stat.WriteTotal(ai, settings, stat.shots, stat.crashes, stat.badShots, stat.gamesPlayed);
            resultsLog.Info(message);
        }
    }
}