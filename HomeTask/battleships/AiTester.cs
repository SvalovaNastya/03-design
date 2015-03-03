using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace battleships
{
    public class AiTester
    {
        public event Action<string> writeLogInfo;
        private readonly Settings settings;

        public AiTester(Settings settings)
        {
            this.settings = settings;
        }

        //dep
        public void TestSingleFile(Ai ai, List<Game> gamesArray)
        {
            var vis = new GameVisualizer();
            var monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds * settings.GamesCount), settings.MemoryLimit);
            var badShots = 0;
            var crashes = 0;
            var gamesPlayed = 0;
            var shots = new List<int>();
            ai.registerProcess += monitor.Register;
            for (var gameIndex = 0; gameIndex < gamesArray.Count; gameIndex++)
            {
                var game = gamesArray[gameIndex];
                RunGameToEnd(game, vis);
                gamesPlayed++;
                badShots += game.BadShots;
                if (game.AiCrashed)
                {
                    crashes++;
                    if (crashes > settings.CrashLimit) break;
                    ai.Dispose();
                    ai.registerProcess += monitor.Register;
                }
                else
                    shots.Add(game.TurnsCount);
                if (settings.Verbose)
                {
                    Console.WriteLine(
                        "Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
                        game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
                }
            }
            ai.Dispose();
            var stat = new Statistic();
            var message = stat.WriteTotal(ai, settings, shots, crashes, badShots, gamesPlayed);
            writeLogInfo(message);
        }

        private void RunGameToEnd(Game game, GameVisualizer vis)
        {
            while (!game.IsOver())
            {
                game.MakeStep();
                if (settings.Interactive)
                {
                    vis.Visualize(game);
                    if (game.AiCrashed)
                        Console.WriteLine(game.LastError.Message);
                    Console.ReadKey();
                }
            }
        }
    }
}