using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    class NullTimeSignature : AbstractTimeSignature
    {
        public NullTimeSignature()
        {
            NumberOfBeats = 4;
            TimeOfBeats = 4;
        }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }

        public override bool isNil()
        {
            return true;
        }
    }
}
