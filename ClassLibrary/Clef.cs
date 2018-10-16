using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Clef : Symbol
    {
        public enum Key { G, F, C};
        public Key key;

        public Clef()
        {

        }

        public Clef(Key key)
        {
            this.key = key;
        }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
