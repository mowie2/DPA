using DomainModel;
using DPA_Musicsheet;
using DPA_Musicsheets.Facade;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Savers;
using DPA_Musicsheets.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace DPA_Musicsheets.Managers
{
    public class MusicController : ViewModelBase
    {
        FileManager fileManager;
        //private string _lilyPondText;
        public IMusicPlayer musicPlayer;
        /*
        public string lilyPondText
        {
            get
            {
                return _lilyPondText;
            }
            set
            {
                _lilyPondText = value;
                base.RaisePropertyChanged("lilyPondText");
            }
        }
        */
        string path;
        public Symbol musicData;
        private PsamContolLib psamContolLib;
        private Editor editor;

        private StaffsViewModel staffsViewModel;
        public MusicController(StaffsViewModel staffs, Editor edit)
        {

            fileManager = new FileManager();
            psamContolLib = new PsamContolLib();
            staffsViewModel = staffs;
            editor = edit;

            musicPlayer = new SanfordLib();
        }

        public void SetMusicPlayer()
        {
            musicPlayer.SetMusic(musicData);
        }

        public void Play()
        {
            //midiPlayer.SetMidisequence(musicData);
            musicPlayer.Continue();
        }

        public void Save()
        {
            fileManager.SaveFile(musicData);
        }

        public void SaveToPDF()
        {
            //fileManager.SaveFile(musicData, ".pdf");
        }

        public string OpenFile()
        {
            path = fileManager.OpenFile();
            return path;
        }

        public string LoadFile()
        {
            musicData = fileManager.LoadFile(path);
            //lilyPondText = fileManager.lilypondText;
            SetMusicPlayer();
            SetStaffs();
            return "";//lilyPondText;
        }

        public void SetStaffs()
        {
            staffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(musicData));
        }

        public void SetStaffs(Symbol symbol)
        {
            staffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(symbol));
        }
    }
}
