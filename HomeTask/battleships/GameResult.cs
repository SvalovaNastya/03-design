using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
    public class GameResult
    {
        public readonly int badShots;
        public readonly int shots;
        public readonly int crashes;

        public GameResult(int badShots, int shots, int crashes)
        {
            this.badShots = badShots;
            this.shots = shots;
            this.crashes = crashes;
        }
    }
}
