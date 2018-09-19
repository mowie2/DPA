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

        protected override Note BuildNote()
        {
            note = new Note();
            note.Clef = this.Clef;
            note.TimeSignature = this.TimeSignature;
            note.Pitch = this.Pitch;
            this.Pitch = "";
            note.Semitone = this.Semitone;
            this.Semitone = ClassLibrary.Semitone.SEMITONE.NORMAL;
            note.Duration = this.Duration;
            this.Duration = 0;
            note.Dotted = this.Dotted;
            this.Dotted = 0;
            return note;
        }

        protected override void SetClef(Clef clef)
        {
            this.Clef = clef;
        }

        protected override void SetTimeSignature(TimeSignature timeSignature)
        {
            this.TimeSignature = timeSignature;
        }

        protected override void SetDotted(int dotted)
        {
            this.Dotted = dotted;
        }

        protected override void SetDuriation(float duration)
        {
            this.Duration = duration;
        }

        protected override void SetPitch(char Pitch)
        {
            this.Pitch = Pitch;
        }

        protected override void SetSemitone(Semitone.SEMITONE semitone)
        {
            this.Semitone = semitone;
        }
    }
}
