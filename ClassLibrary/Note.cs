using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Note : Symbol
    {
        public Clef Clef { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public string Pitch { get; set; }
        public Semitone.SEMITONE Semitone { get; set; }
        public float Duration { get; set; }
        public int Octave { get; set; }
        public int Dotted { get; set; }
        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
