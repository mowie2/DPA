using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Note
    {
        public Clef Clef { get; set; }
        public char Pitch { get; set; }
        public Semitone.SEMITONE Semitone { get; set; }
        public float Duration { get; set; }
        public int Dotted { get; set; }
    }
}
