using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLassLibrary
{
    public abstract class Builder
    {
        protected Clef clef;
        protected TimeSignature timeSignature;

        protected abstract void SetDuriation(float duration);
        protected abstract void SetPitch(char pitch);
        protected abstract void SetSemitone(Semitone.SEMITONE semitone);
        protected abstract void SetClef(Clef clef);
        protected abstract void SetClef(TimeSignature timeSignature);
        protected abstract Note BuildNote();


    }
}
