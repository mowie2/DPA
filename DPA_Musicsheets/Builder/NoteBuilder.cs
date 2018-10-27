using DomainModel;

namespace DPA_Musicsheets.Builders
{
    public class NoteBuilder
    {
        private Note note;
        private Clef clef;
        private TimeSignature timeSignature;
        private Tempo tempo;

        private string pitch;
        private Semitone.SEMITONE semitone;
        private int ocataveModifier;
        private float duration;
        private int dotted;

        public NoteBuilder()
        {
            this.semitone = DomainModel.Semitone.SEMITONE.NORMAL;
            this.dotted = 0;
            this.duration = 0;
            this.ocataveModifier = 0;
            this.semitone = Semitone.SEMITONE.NORMAL;
        }
        public Note BuildNote()
        {
            note = new Note
            {
                Clef = this.clef,
                TimeSignature = this.timeSignature,
                Tempo = this.tempo,
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
            this.semitone = DomainModel.Semitone.SEMITONE.NORMAL;
            this.pitch = "";
        }
        public void SetClef(Clef clef)
        {
            this.clef = clef;
        }

        public void SetTempo(Tempo tempo)
        {
            this.tempo = tempo;
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

        public void ClearOctave()
        {
            this.ocataveModifier = 0;
        }
    }
}
