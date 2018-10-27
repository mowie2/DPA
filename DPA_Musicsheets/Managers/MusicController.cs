using ClassLibrary;
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

namespace DPA_Musicsheets.Managers
{
    public class MusicController : ViewModelBase
    {
        FileManager fileManager;
        //public MidiPlayerViewModel midiPlayerView;
        private string _lilyPondText;
        public IMidiPlayer midiPlayer;

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

        string path;
        public Symbol musicData;
        private PsamContolLib psamContolLib;
        private Editor editor;
        private StaffsViewModel staffsViewModel;
        public MusicController(StaffsViewModel staffs, Editor edit)
        {
            //musicData = ml.LilypondText;
            fileManager = new FileManager();
            psamContolLib = new PsamContolLib();
            staffsViewModel = staffs;
            editor = edit;

            midiPlayer = new SanfordLib();
            //path = "C:\\Users\\mo\\Desktop\\School\\DPA\\DPA_Musicsheets\\Files\\Herhaling_metAlternatief.ly";
            //musicData = fileManager.LoadFile(path);
            //Test();
        }

        void Test()
        {
            musicData = new Note()
            {
                Pitch = "B",
                Duration = 8,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },

                Clef = new Clef(Clef.Key.G)

            };
            musicData.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 8,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
            musicData.nextSymbol.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 4,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
            musicData.nextSymbol.nextSymbol.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 4,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
            musicData.nextSymbol.nextSymbol.nextSymbol.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 4,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
            musicData.nextSymbol.nextSymbol.nextSymbol.nextSymbol.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 4,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
            musicData.nextSymbol.nextSymbol.nextSymbol.nextSymbol.nextSymbol.nextSymbol = new Note()
            {
                Pitch = "B",
                Duration = 4,
                TimeSignature = new TimeSignature()
                {
                    NumberOfBeats = 4,
                    TimeOfBeats = 4
                },
                Clef = new Clef(Clef.Key.G)
            };
        }

        public void Play()
        {
            //midiPlayer.SetMidisequence(musicData);
            midiPlayer.ContinueSequence();
        }

        public void Save()
        {
            fileManager.SaveFile(musicData);
        }

        public void OpenFile()
        {
            path = fileManager.OpenFile();
        }

        public string LoadFile()
        {
            musicData = fileManager.LoadFile(path);
            lilyPondText = fileManager.lilypondText;
            midiPlayer.SetMidisequence(musicData);
            SetStaffs();
            return lilyPondText;
        }

        public void SetStaffs()
        {
            //LoadFile();
            staffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(musicData));
        }

        public void SetStaffs(Symbol symbol)
        {
            staffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(symbol));
        }
    }
}
