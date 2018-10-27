using DomainModel;
using DPA_Musicsheets.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DPA_Musicsheet
{
    public class FileManager
    {
        //private Dictionary<string, ISavable> savables;
        //private readonly Dictionary<string, IReader> readers;
        private readonly List<IReader> readers;
        private readonly List<ISavable> savables;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        public string lilypondText;


        public FileManager()
        {
            var type = typeof(IReader);
            IEnumerable<Type> assemblies;
            var spath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblies = Directory.GetFiles(spath, "*.dll")
                .Select(dll => Assembly.LoadFile(dll))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.IsPublic && !p.IsAbstract);
            readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IReader)Activator.CreateInstance(c)).ToList();

            type = typeof(ISavable);
            savables = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (ISavable)Activator.CreateInstance(c)).ToList();


            //savables = new Dictionary<string, ISavable>
            //{
            //{ ".pdf", new SaveToPDF() },
            //{ ".ly", new SaveToLily() },
            //{ ".mid", new SaveToMidi() }
            //};

            //readers = new Dictionary<string, IReader>()
            //{
            //{".mid", new MidiReader() },
            //{".ly", new LillyPondReader() }
            //};

            openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
        }

        internal Symbol LoadFile(string path)
        {
            string extension = Path.GetExtension(openFileDialog.FileName);

            List<IReader> readerWithExtention = readers.Where(p => p.GetExtention().Equals(extension)).ToList();

            if (readerWithExtention.Count >0)
            {
                IReader reader = readerWithExtention[0];
                string fileName = openFileDialog.FileName;
                Symbol root = reader.readFile(fileName);
                lilypondText = reader.GetMusicText();
                return root;
            }
            return null;
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

        public void SaveFile(Symbol musicData)
        {

            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);

                List<ISavable> saverWithExtention = savables.Where(p => p.GetExtention().Equals(extension)).ToList();

                if (saverWithExtention.Count() == 0)
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                    return;
                }

                ISavable saver = saverWithExtention[0];
                saver.Save(saveFileDialog.FileName, musicData);
            }

        }
    }
}
