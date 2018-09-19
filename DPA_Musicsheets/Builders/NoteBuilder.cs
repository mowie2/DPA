using ClassLibrary;
using CLassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Builders
{
    class NoteBuilder : AbstractNoteBuilder
    {
        protected override Note BuildNote()
        {
            PreviousNote.NextNote = NewNote;
            PreviousNote = NewNote;
            NewNote = new Note();
            return PreviousNote;
        }

        protected override void SetClef(Clef clef)
        {
            NewNote.Clef = clef;
        }

        protected override void SetTimeSignature(TimeSignature timeSignature)
        {
            NewNote.TimeSignature = timeSignature;
        }

        protected override void SetDotted(int dotted)
        {
            NewNote.Dotted = dotted;
        }

        protected override void SetDuriation(float duration)
        {
            NewNote.Duration = duration;
        }

        protected override void SetPitch(char Pitch)
        {
            NewNote.Pitch = Pitch;
        }

        protected override void SetSemitone(Semitone.SEMITONE semitone)
        {
            NewNote.Semitone = semitone;
        }
    }
}
