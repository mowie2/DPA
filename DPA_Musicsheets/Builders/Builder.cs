using ClassLibrary;
using CLassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Builders
{
    class Builder : AbstractBuilder
    {
        protected override Note BuildNote()
        {
            PreviousNote = new Note();
            PreviousNote.Clef = Clef;
            PreviousNote;

            return PreviousNote;
        }

        protected override void SetClef(Clef clef)
        {
            throw new NotImplementedException();
        }

        protected override void SetClef(TimeSignature timeSignature)
        {
            throw new NotImplementedException();
        }

        protected override void SetDotted(int Dotted)
        {
            throw new NotImplementedException();
        }

        protected override void SetDuriation(float duration)
        {
            throw new NotImplementedException();
        }

        protected override void SetPitch(char Pitch)
        {
            throw new NotImplementedException();
        }

        protected override void SetSemitone(Semitone.SEMITONE semitone)
        {
            throw new NotImplementedException();
        }
    }
}
