using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Clef : AbstractClef
    {
        public Clef()
        {
            this.key = Key.NOTSET;
        }

        public Clef(Key key)
        {
            this.key = key;
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
