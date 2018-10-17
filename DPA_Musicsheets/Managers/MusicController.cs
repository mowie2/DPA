using DPA_Musicsheet;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Managers
{
    public class MusicController
    {
        FileReader fileReader;
        Dictionary<string, IReader> readers;
        object musicData;

        public MusicController()
        {
            fileReader = new FileReader();
            readers = new Dictionary<string, IReader>()
            {
                {"mid", new MidiReader() }
            };

        }
        public void Save(string extension, string fileName)
        {
            fileReader.SaveFile(extension, fileName, musicData);
        }
    }
}
