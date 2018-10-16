using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Savers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DPA_Musicsheet
{
    public class FileReader
    {
        public Dictionary<string, ISavable> savables;
        
        public FileReader()
        {
            savables = new Dictionary<string, ISavable>();
            savables.Add(".pdf", new SaveToPDF());
        }
        
        public string OpenFile()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog.FileName;
                } else
                {
                    return null;
                }
            } catch (Exception e)
            {
                MessageBox.Show("Could not open the selected file because of the folowing error: " + e);
                return null;
                
            }
        }

        public void SaveFile(string extension, string fileName, object musicData)
        {
            if (!savables.ContainsKey(extension))
            {
                MessageBox.Show($"Extension {extension} is not supported.");
            }
            ISavable saver = savables[extension];
            saver.Save(fileName, musicData);
            
        }
    }
}
