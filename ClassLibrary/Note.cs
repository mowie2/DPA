using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Note : Symbol
    {
        private AbstractClef _Clef;
        public AbstractClef Clef {
            get
            {
                if (this._Clef == null)
                    return new NullClef();
                return _Clef;
            }

            set
            {
                if (value == null)
                    return;
                _Clef = value;
            }
        }
        private AbstractTimeSignature _TimeSignature;
        public AbstractTimeSignature TimeSignature
        {
            get
            {
                if (this._TimeSignature == null)
                    return new NullTimeSignature();
                return _TimeSignature;
            }

            set
            {
                if (value == null)
                    return;
                _TimeSignature = value;
            }
        }
        private AbstractTempo _Tempo;
        public AbstractTempo Tempo {
            get
            {
                if (this.Tempo == null)
                    return new NullTempo();
                return _Tempo;
            }

            set
            {
                if (value == null)
                    return;
                _Tempo = value;
            }
        }
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
