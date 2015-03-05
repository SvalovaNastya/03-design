using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
    public class ConsVisualiser
    {
        public void WriteGameStat(Game game, int gameIndex)
        {
            Console.WriteLine(
                "Game #{3,4}: Turns {0,4}, BadShots {1}{2}",
                game.TurnsCount, game.BadShots, game.AiCrashed ? ", Crashed" : "", gameIndex);
        }
    }
}
