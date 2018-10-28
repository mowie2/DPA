using DomainModel;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Savers
{
    public class SaveToPDF// : ISavable
    {
        public void Save(string fileName, Symbol note)
        {
            string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string tmpFileName = $"{fileName}-tmp.ly";
            //write(tmpFileName, musicData);

            string lilypondLocation = @"C:\Program Files (x86)\LilyPond\usr\bin\lilypond.exe";
            string sourceFolder = Path.GetDirectoryName(tmpFileName);
            string sourceFileName = Path.GetFileNameWithoutExtension(tmpFileName);
            string targetFolder = Path.GetDirectoryName(fileName);
            string targetFileName = Path.GetFileNameWithoutExtension(fileName);

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = sourceFolder,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = String.Format("--pdf \"{0}\\{1}.ly\"", sourceFolder, sourceFileName),
                    FileName = lilypondLocation
                }
            };

            process.Start();
            while (!process.HasExited)
            { /* Wait for exit */
            }
            if (sourceFolder != targetFolder || sourceFileName != targetFileName)
            {
                var source = sourceFolder + "\\" + sourceFileName + ".pdf";
                var destination = targetFolder + "\\" + targetFileName + ".pdf";
                File.Move(source, destination);
                File.Delete(tmpFileName);
            }
        }
    
        private void write(string fileName, Symbol musicData)
        {
            /*
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
               // outputFile.Write((string)musicData);
                outputFile.Close();
            }
            */
        }
    }
}
