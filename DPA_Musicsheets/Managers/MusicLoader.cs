using DPA_Musicsheets.ViewModels;
using PSAMControlLibrary;
using PSAMWPFControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Managers
{
    /// <summary>
    /// This is the one and only god class in the application.
    /// It knows all about all file types, knows every viewmodel and contains all logic.
    /// TODO: Clean this class up.
    /// </summary>
    public class MusicLoader
    {
        #region Properties
        public string LilypondText { get; set; }
        public List<MusicalSymbol> WPFStaffs { get; set; } = new List<MusicalSymbol>();
        private static List<Char> notesorder = new List<Char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };

        //public Sequence MidiSequence { get; set; }
        #endregion Properties

        private int _beatNote = 4;    // De waarde van een beatnote.
        private int _bpm = 120;       // Aantal beatnotes per minute.
        private int _beatsPerBar;     // Aantal beatnotes per maat.

        public MainViewModel MainViewModel { get; set; }
        public StaffsViewModel StaffsViewModel { get; set; }

        /// <summary>
        /// Opens a file.
        /// TODO: Remove the switch cases and delegate.
        /// TODO: Remove the knowledge of filetypes. What if we want to support MusicXML later?
        /// TODO: Remove the calling of the outer viewmodel layer. We want to be able reuse this in an ASP.NET Core application for example.
        /// </summary>
        /// <param name="fileName"></param>
        /*
        public void OpenFile(string fileName)
        {
            if (Path.GetExtension(fileName).EndsWith(".mid"))
            {
                MidiPlayerViewModel.slb.CreateSequence(fileName);

                // MidiPlayerViewModel.slb.MidiSequence = MidiSequence;
                //this.LilypondText = LoadMidiIntoLilypond(MidiPlayerViewModel.slb.MidiSequence);
                this.LilypondViewModel.LilypondTextLoaded(this.LilypondText);
            }
            else if (Path.GetExtension(fileName).EndsWith(".ly"))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var line in File.ReadAllLines(fileName))
                {
                    sb.AppendLine(line);
                }
                
                this.LilypondText = sb.ToString();
                this.LilypondViewModel.LilypondTextLoaded(this.LilypondText);
            }
            else
            {
                throw new NotSupportedException($"File extension {Path.GetExtension(fileName)} is not supported.");
            }

            //LillyPondReader tokenizer = new LillyPondReader();
            //tokenizer.ReadLily(LilypondText);

            //LoadLilypondIntoWpfStaffsAndMidi(LilypondText);
        }
        */
    }
}
