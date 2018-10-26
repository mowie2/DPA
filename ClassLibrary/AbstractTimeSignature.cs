using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public abstract class AbstractTimeSignature : Symbol
    {
        public int NumberOfBeats { get; set; }
        public int TimeOfBeats { get; set; }

        public abstract bool isNil();
    }
}
