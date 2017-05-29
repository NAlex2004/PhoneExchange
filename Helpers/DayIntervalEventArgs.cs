using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAlex.Helpers
{
    public class DayIntervalEventArgs: EventArgs
    {
        public int OldInterval;
        public int NewInterval;
    }
}
