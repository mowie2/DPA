using ClassLibrary;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Savers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DPA_Musicsheet
{
    public class FileManager
    {
        private Dictionary<string, ISavable> savables;
        private readonly Dictionary<string, IReader> readers;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;


        public FileManager()
        {
            savables = new Dictionary<string, ISavable>
            {
                { ".pdf", new SaveToPDF() },
                { ".ly", new SaveToLily() },
                { ".mid", new SaveToMidi() }
            };

            readers = new Dictionary<string, IReader>()
            {
                {"mid", new MidiReader() }
            };

            openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
        }

        internal Note LoadFile(string path)
        {
            string extension = Path.GetExtension(openFileDialog.FileName);
            IReader reader = readers[extension];
            return reader.readFile(openFileDialog.FileName);
        }

        public string OpenFile()
        {

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not open the selected file because of the folowing error: " + e);
                return null;

            }
        }

        public void SaveFile(Note musicData)
        {

            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (!savables.ContainsKey(extension))
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                    return;
                }

                ISavable saver = savables[extension];
                saver.Save(saveFileDialog.FileName, musicData);

            }

        }
    }
}
