using ClassLibrary.Interfaces;
using DomainModel;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DPA_Musicsheets.Managers
{
    public class Editor : ViewModelBase
    {
        private IConvertToExtention converter;

        public Editor()
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
        public string TextChanged(Symbol symbol)
        {
            //return "test"; 
            if (converter == null) return "";
            return converter.Convert(symbol) as string;
        }
    }
}
