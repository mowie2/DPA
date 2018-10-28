using ClassLibrary.Interfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LilypondAdapter
{
    public class SaveToLily : ISavable
    {
        public string extention = ".ly";
        private IConvertToExtention converter;

        public SaveToLily()
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
            if (converters.Count > 0)
            {
                converter = converters[0];
            }
        }

        public void Save(string fileName, Symbol root)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                if (converter == null) return;
                outputFile.Write(converter.Convert(root));
                outputFile.Close();
            }       
        }
        public string GetExtention()
        {
            return extention;
        }
    }
}
