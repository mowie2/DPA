using ClassLibrary;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Savers
{
    public class SaveToMidi : ISavable
    {
        public void Save(string fileName, Note note)
        {
            Note currentNote = note;
            while (currentNote != null)
            {
                setPitch(note.Pitch, note.Octave, note.Clef, note.Semitone);
                setDuration(note.Duration, note.Dotted, note.TimeSignature);
            }
        }
    }
}
