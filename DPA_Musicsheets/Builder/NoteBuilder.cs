using ClassLibrary;

namespace DPA_Musicsheets.Builders
{
    public class NoteBuilder
    {
        private Note note;
        private Clef clef;
        private TimeSignature timeSignature;

        private string pitch;
        private Semitone.SEMITONE semitone;
        private int ocataveModifier;
        private float duration;
        private int dotted;

        public NoteBuilder()
        {
            this.dotted = 0;
            this.duration = 0;
            this.ocataveModifier = 0;
        }
        public Note BuildNote()
        {
            note = new Note
            {
                Clef = this.clef,
                TimeSignature = this.timeSignature,
                Pitch = this.pitch,
                Semitone = this.semitone,
                Octave = this.ocataveModifier,
                Duration = this.duration,
                Dotted = this.dotted
            };

            Clear();
            return note;
        }

        private void Clear()
        {
            this.dotted = 0;
            this.duration = 0;
            this.semitone = ClassLibrary.Semitone.SEMITONE.NORMAL;
            this.pitch = "";
        }
        public void SetClef(Clef clef)
        {
            this.clef = clef;
        }

        public void SetTimeSignature(TimeSignature timeSignature)
        {
            this.timeSignature = timeSignature;
        }

        public void SetDotted(int dotted)
        {
            this.dotted = dotted;
        }

        public void SetDuriation(float duration)
        {
            this.duration = duration;
        }

        public void SetPitch(string Pitch)
        {
            this.pitch = Pitch;
        }

        public void SetSemitone(Semitone.SEMITONE semitone)
        {
            this.semitone = semitone;
        }

        public void ModifyOctave(int octaveModifier)
        {
            this.ocataveModifier += octaveModifier;
        }
    }
}
