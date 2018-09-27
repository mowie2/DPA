using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class TimeSignature : Symbol
    {
        public int NumberOfBeats { get; set; }
        public int TimeOfBeats { get; set; }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
