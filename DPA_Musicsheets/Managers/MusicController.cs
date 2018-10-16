using DPA_Musicsheet;
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
        object musicData;

        public MusicController()
        {
            fileReader = new FileReader();
        }
        public void Save(string extension, string fileName)
        {
            fileReader.SaveFile(extension, fileName, musicData);
        }
    }
}
