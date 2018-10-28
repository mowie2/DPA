using DomainModel;
using DPA_Musicsheets.Interfaces;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace SanfordAdapter
{
    public class SaveToMidi : ISavable
    {
        private readonly string fancyName = "MIDI";
        private readonly string extention = ".mid";

        public void Save(string fileName, Symbol symbol)
        {
            DomainToMidi dm = new DomainToMidi();
            Sequence s = dm.Convert(symbol) as Sequence;
            s.Save(fileName);
        }

        public string GetExtention()
        {
            return extention;
        }

        public string GetFancyName()
        {
            return fancyName;
        }
    }
}
