using DPA_Musicsheets.Interfaces;
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
        public Dictionary<string, ISavable> savables;

        public FileManager()
        {
            savables = new Dictionary<string, ISavable>
            {
                //{ ".pdf", new SaveToPDF() },
                { ".ly", new SaveToLily() },
                { ".mid", new SaveToMidi() }
            };
        }

        public string OpenFile()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
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

        public void SaveFile(object musicData)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };

            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (savables.ContainsKey(extension))
                {
                    ISavable saver = savables[extension];
                    //saver.Save(saveFileDialog.FileName, musicData);
                }
                else
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                }


            }



        }
    }
}
