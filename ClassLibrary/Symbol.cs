using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary
{
    public abstract class Symbol : MusicSymbol
    {
        public Symbol nextSymbol { get; set; }
    }
}