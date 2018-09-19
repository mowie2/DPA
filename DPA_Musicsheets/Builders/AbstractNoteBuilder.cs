using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Builders
{
    public abstract class AbstractNoteBuilder
    {
        protected Note note;
        protected Clef Clef;
        protected TimeSignature TimeSignature;

        protected string Pitch;
        protected Semitone.SEMITONE Semitone;
        protected float Duration;
        protected int Dotted;


        protected abstract void SetClef(Clef clef);
        protected abstract void SetTimeSignature(TimeSignature timeSignature);
        protected abstract void SetPitch(String Pitch);
        protected abstract void SetSemitone(Semitone.SEMITONE semitone);
        protected abstract void SetDuriation(float duration);
        protected abstract void SetDotted(int Dotted);

        protected abstract Note BuildNote();


    }
}
