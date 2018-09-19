using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Builders
{
    class NoteBuilder : AbstractNoteBuilder
    {

        public NoteBuilder()
        {
            note = new Note();
        }
        protected override Note BuildNote()
        {
            
            return note;
        }

        protected override void SetClef(Clef clef)
        {
            this.Clef = clef;
        }

        protected override void SetTimeSignature(TimeSignature timeSignature)
        {
            note.TimeSignature = timeSignature;
        }

        protected override void SetDotted(int dotted)
        {
            note.Dotted = dotted;
        }

        protected override void SetDuriation(float duration)
        {
            note.Duration = duration;
        }

        protected override void SetPitch(char Pitch)
        {
            note.Pitch = Pitch;
        }

        protected override void SetSemitone(Semitone.SEMITONE semitone)
        {
            note.Semitone = semitone;
        }
    }
}
