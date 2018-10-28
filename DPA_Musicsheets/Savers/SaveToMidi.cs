using ClassLibrary;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Interfaces;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Savers
{
    public class SaveToMidi : ISavable
    {
        public void Save(string fileName, Symbol symbol)
        {
            DomainToMidi dm = new DomainToMidi();
            Sequence sequence = dm.GetMidiSequence(symbol);
            sequence.Save(fileName);
        }
    }
}
