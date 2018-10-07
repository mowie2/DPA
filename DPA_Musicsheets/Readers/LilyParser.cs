using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Readers
{
    class LilyParser
    {
        private Builders.NoteBuilder noteBuilder = new Builders.NoteBuilder();
        public void FindClef(string text)
        {
            Clef tempClef = new Clef();
            switch (text)
            {
                case "treble":
                    tempClef.key = Clef.Key.G;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "bass":
                    tempClef.key = Clef.Key.F;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "alto":
                    tempClef.key = Clef.Key.C;
                    noteBuilder.SetClef(tempClef);
                    break;
            }
        }

        public void FindTimeSignature(int numberOfBeats,int timeOfBeats)
        {
            TimeSignature t = new TimeSignature
            {
                NumberOfBeats = numberOfBeats,
                TimeOfBeats = timeOfBeats
            };
            noteBuilder.SetTimeSignature(t);
        }
        public void FindNote(string pitch,string pitchModifier,string octaveModifier,int duration,int dotted)
        {
            noteBuilder.SetPitch(pitch);
            //noteBuilder.setPitchModifier(pitchModifier);
            //noteBuilder.setOctaveModifier(octaveModifier);
            noteBuilder.SetDuriation((float)duration);
            noteBuilder.SetDotted(dotted);
        }

        public Note getNote()
        {
            return noteBuilder.BuildNote();
        }
    }
}
