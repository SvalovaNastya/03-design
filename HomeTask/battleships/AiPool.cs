using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleships
{
    public class AiPool
    {
        private int limit;
        private ProcessMonitor monitor;
        private string exe;

        public AiPool(int limit, ProcessMonitor monitor, string exe)
        {
            this.limit = limit + 1;
            this.exe = exe;
            this.monitor = monitor;
        }

        public Ai GetNewAi()
        {
            if (limit < 0)
                throw new Exception("settings.CrashLimit ended");
            limit--;
            var ai = new Ai(exe);
            ai.registerProcess += monitor.Register;
            return ai;
        }
    }
}
