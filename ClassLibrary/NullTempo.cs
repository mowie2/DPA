using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class NullTempo : AbstractTempo
    {
        public NullTempo()
        {
            bpm = 120;
            noteDuration = 4;
        }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }

        public override bool isNil()
        {
            return false;
        }
    }
}
