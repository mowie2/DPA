using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public abstract class AbstractClef : Symbol
    {
        public enum Key { G, F, C, NOTSET };
        public Key key;

        public abstract bool isNil();
    }
}
