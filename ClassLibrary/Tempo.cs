using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Tempo : Symbol
    {
        public int bpm { get; set; }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
