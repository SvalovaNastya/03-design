using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NLog;

namespace battleships
{
    public class AiTester
    {
        private readonly Settings settings;

        public AiTester(Settings settings)
        {
            this.settings = settings;
        }

        public Statistic RunOneGame(Game game, Action<Game> visualize, int gameIndex, Statistic stat)
        {
            RunGameToEnd(game, visualize);
            var newShots = new List<int>(stat.shots);
            var crashes = stat.crashes;
            if (game.AiCrashed)
            {
                crashes++;
                if (crashes > settings.CrashLimit)
                    return new Statistic(stat.badShots + game.BadShots, stat.shots, crashes, stat.gamesPlayed + 1);
//                ai.Dispose();
            }
            else
                newShots.Add(game.TurnsCount);
            if (settings.Verbose)
            {
                Console.WriteLine(
                    "Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
                    game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
            }
            return new Statistic(stat.badShots + game.BadShots, newShots, crashes, stat.gamesPlayed + 1);
        }

        private void RunGameToEnd(Game game, Action<Game> visualize)
        {
            while (!game.IsOver())
            {
                game.MakeStep();
                if (settings.Interactive)
                {
                    visualize(game);
                    if (game.AiCrashed)
                        Console.WriteLine(game.LastError.Message);
                    Console.ReadKey();
                }
            }
        }
    }
}