using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Note : Symbol
    {
        enum SEMITONE { MAJOR, NORMAL, MINOR };
        private char Pitch;
        private float Duration;
        private SEMITONE Semitone;
        private int Octaaf;
        private int Dotted;

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
