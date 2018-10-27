using DomainModel;
using DPA_Musicsheet;
using DPA_Musicsheets.Facade;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DPA_Musicsheets.Managers
{
    public class MusicController : ViewModelBase
    {
        FileManager fileManager;
        private string _lilyPondText;
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
        private MusicLoader musicLoader;
        private Editor editor;
        public MusicController(MusicLoader ml, Editor edit)
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
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();


            var type = typeof(IReader);
            var test = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Select(x => x.Name).ToList();

            var type2 = typeof(ISavable);
            var test2 = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(p => type2.IsAssignableFrom(p))
                .Select(x => x.Name).ToList();

            //musicData = ml.LilypondText;
            fileManager = new FileManager();
            psamContolLib = new PsamContolLib();
            musicLoader = ml;
            editor = edit;
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
            lilyPondText = fileManager.lilypondText;
            SetStaffs();
        }

        public void SetStaffs()
        {
            //LoadFile();
            musicLoader.StaffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(musicData));
        }

        public void SetStaffs(Symbol symbol)
        {
            musicLoader.StaffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(symbol));
        }
    }
}
