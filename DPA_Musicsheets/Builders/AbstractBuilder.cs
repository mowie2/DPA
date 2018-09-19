using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLassLibrary
{
    public abstract class AbstractBuilder
    {
        protected Note PreviousNote;
        protected Clef Clef;
        protected TimeSignature TimeSignature;
        protected char Pitch;
        protected Semitone.SEMITONE Semitone;
        protected float Duration;
        protected int Dotted;

        protected abstract void SetClef(Clef clef);
        protected abstract void SetClef(TimeSignature timeSignature);
        protected abstract void SetPitch(char Pitch);
        protected abstract void SetSemitone(Semitone.SEMITONE semitone);
        protected abstract void SetDuriation(float duration);
        protected abstract void SetDotted(int Dotted);

        protected abstract Note BuildNote();


    }
}
