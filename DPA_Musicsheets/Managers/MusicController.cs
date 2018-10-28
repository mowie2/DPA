﻿using DomainModel;
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
        //public MidiPlayerViewModel midiPlayerView;
        private string _lilyPondText;
        public IMusicPlayer musicPlayer;

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
        private IEnumerable<Type> assemblies;

        private StaffsViewModel staffsViewModel;
        public MusicController(StaffsViewModel staffs, Editor edit)
        {

            #region uitleg
            ///Als je hieraan gaat werken:
            ///Bouw LilypondAdapter en SanfordAdapter.
            ///Bij DPAMusicsheets->References rechtermuis, add references.
            ///Ga naar browse en dan de lilypondAdapter.dll en SanfordAdapter.dll opzoeken en die toevoegen.
            /// 
            /// getAsseblies() moet alle adapters laden maar doet dat niet
            ///Als je een klasse maakt uit de adapter doet hij het wel,
            ///maar dit is niet netjes. Dan is het namelijk hardcoded
            /// 
            /// links die ik gebruikt heb:
            ///https://blogs.msdn.microsoft.com/benjaminperkins/2017/04/13/how-to-make-a-simple-dll-as-an-assembly-reference-just-for-fun/
            ///Hieronder ben ik gewoon de get aan het testen, kijken of ik de midiReader en lilypondReader terug kan krijgen
            #endregion
            /*
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            //var l = new LilypondAdapter.LilypondReader();

            var type = typeof(IReader);
            var spath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblies = Directory.GetFiles(spath, "*.dll")
                .Select(dll => Assembly.LoadFile(dll))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.IsPublic && !p.IsAbstract);

            var readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IReader)Activator.CreateInstance(c)).ToList();
            readers = readers.Where(p => p.GetExtention().Equals(".ly")).ToList();

            var type2 = typeof(ISavable);
            var test2 = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(p => p.IsAssignableFrom(type2) && !p.IsInterface)
                .Select(x => x.Name).ToList();
            */
            //musicData = ml.LilypondText;
            fileManager = new FileManager();
            psamContolLib = new PsamContolLib();
            staffsViewModel = staffs;
            editor = edit;

            musicPlayer = new SanfordLib();
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

        public void SetMidiPlayer()
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
            lilyPondText = fileManager.lilypondText;
            SetMidiPlayer();
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
