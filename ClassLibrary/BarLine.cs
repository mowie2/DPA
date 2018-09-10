using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class BarLine : Symbol
    {

        enum TYPE { FINAL, REPEAT, NORMAL, START }
        private BarLine Buddy;
        private bool HasBeenPlayed;
        private TYPE Type;
        private List<Alternative> Alternatives;

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
