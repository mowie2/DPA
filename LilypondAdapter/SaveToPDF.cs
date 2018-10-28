using ClassLibrary.Interfaces;
using DomainModel;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LilypondAdapter
{
    public class SaveToPDF : ISavable
    {
        private string extention;
        private readonly string fancyName = "PDF";
        private IConvertToExtention converter;

        public SaveToPDF()
        {
            IEnumerable<Type> assemblies;
            var type = typeof(IConvertToExtention);
            var spath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblies = Directory.GetFiles(spath, "*.dll")
                .Select(dll => Assembly.LoadFile(dll))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.IsPublic && !p.IsAbstract);

            var converters = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IConvertToExtention)Activator.CreateInstance(c)).ToList();
            converters = converters.Where(p => p.GetExtention().Equals(".ly")).ToList();
            if (converters.Count>0)
            {
                converter = converters[0];
            }
            extention = ".pdf";
        }

        public void Save(string fileName, Symbol musicData)
        {
            string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string tmpFileName = $"{fileName}-tmp.ly";
            write(tmpFileName, musicData);

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
            
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                if (converter == null) return;
                outputFile.Write(converter.Convert(musicData));
                outputFile.Close();
            }
        }

        public string GetExtention()
        {
            return extention;
        }

        public string GetFancyName()
        {
            return fancyName;
        }
    }
}
