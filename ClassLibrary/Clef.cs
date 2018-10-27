using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Clef : Symbol
    {
        public enum Key { G, F, C, NOTSET};
        public Key key;

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
    }
}
