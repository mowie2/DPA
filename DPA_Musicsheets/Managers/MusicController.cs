using ClassLibrary;
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
        FileManager fileManager;
        string path;
        Dictionary<string, IReader> readers;
        Note musicData;

        public MusicController(MusicLoader ml)
        {
            //musicData = ml.LilypondText;
            fileManager = new FileManager();
        }
        public void Save()
        {
            fileManager.SaveFile(musicData);
        }

        public void OpenFile()
        {
            path = fileManager.OpenFile();
        }

        public void LoadFile()
        {
            musicData = fileManager.LoadFile(path);
        }
    }
}
