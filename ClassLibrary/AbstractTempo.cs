using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public abstract class AbstractTempo : Symbol
    {
        public int noteDuration { get; set; }
        public int bpm { get; set; }

        public abstract bool isNil();
    }
}
