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
                    MessageBox.Show("Dialogbox is not showing");
                    return null;
                }
            } catch (Exception e)
            {
                MessageBox.Show("Could not open the selected file because of the folowing error: " + e);
                return null;
                
            }
        }
    }
}
