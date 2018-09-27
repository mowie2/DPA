using ClassLibrary;

namespace DPA_Musicsheets.Builders
{
    public class NoteBuilder
    {
        private Note note;
        private Clef Clef;
        private TimeSignature TimeSignature;

        private  string Pitch;
        private  Semitone.SEMITONE Semitone;
        private  float Duration;
        private  int Dotted;

        public Note BuildNote()
        {
            note = new Note
            {
                Clef = this.Clef,
                TimeSignature = this.TimeSignature,
                Pitch = this.Pitch,
                Semitone = this.Semitone,
                Duration = this.Duration,
                Dotted = this.Dotted
            };

            Clear();
            return note;
        }

        private void Clear()
        {
            this.Dotted = 0;
            this.Duration = 0;
            this.Semitone = ClassLibrary.Semitone.SEMITONE.NORMAL;
            this.Pitch = "";
        }
        public void SetClef(Clef clef)
        {
            this.Clef = clef;
        }

        public void SetTimeSignature(TimeSignature timeSignature)
        {
            this.TimeSignature = timeSignature;
        }

        public void SetDotted(int dotted)
        {
            this.Dotted = dotted;
        }

        public void SetDuriation(float duration)
        {
            this.Duration = duration;
        }

        public void SetPitch(string Pitch)
        {
            this.Pitch = Pitch;
        }

        public void SetSemitone(Semitone.SEMITONE semitone)
        {
            this.Semitone = semitone;
        }
    }
}
