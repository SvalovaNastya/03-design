using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NLog;

namespace battleships
{
    public class GameRunner
    {
        private readonly Settings settings;

        public GameRunner(Settings settings)
        {
            this.settings = settings;
        }

        public GameResult GameRun(Game game, Action<Game> actionOnEveryStep)
        {
            RunGameToEnd(game, actionOnEveryStep);
            var shot = 0;
            var crashes = 0;
            if (game.AiCrashed)
            {
                crashes++;
//                if (crashes > settings.CrashLimit)
//                    return new GameResult(game.BadShots, 0, crashes);
//                AiPool a = new AiPool();
//                ai.Dispose();
            }
            else
                shot = game.TurnsCount;
            return new GameResult(game.BadShots, shot, crashes);
        }

        private void RunGameToEnd(Game game, Action<Game> actionOnEveryStep)
        {
            while (!game.IsOver())
            {
                game.MakeStep();
                if (settings.Interactive)
                {
                    actionOnEveryStep(game);
                    if (game.AiCrashed)
                        Console.WriteLine(game.LastError.Message);
                    Console.ReadKey();
                }
            }
        }
    }
}