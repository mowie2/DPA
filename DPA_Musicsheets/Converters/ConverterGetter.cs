using ClassLibrary.Interfaces;
using DomainModel;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converters
{
    public class ConverterGetter
    {
        private readonly IEnumerable<Type> assemblies;
        public ConverterGetter()
        {
            var spath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblies = Directory.GetFiles(spath, "*.dll")
                .Select(dll => Assembly.LoadFile(dll))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.IsPublic && !p.IsAbstract);
        }

        /*
        public object GetConverter(object i,string extention)
        {
            return null;
            
            var type = i.GetType();
            var readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (c.GetType())Activator.CreateInstance(c)).ToList();
            if (readers.count() > 0) return null;
            return readers[0];
        }
        */

        public List<string> GetSupportOpenTypes()
        {
            var type = typeof(IReader);
            var readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IReader)Activator.CreateInstance(c)).ToList();
            List<string> types = readers.Select(p => p.GetExtention()).ToList();



            return types;

        }

        public List<string> GetSupportCloseTypes()
        {
            var type = typeof(ISavable);
            var readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (ISavable)Activator.CreateInstance(c)).ToList();
            List<string> types = readers.Select(p => p.GetExtention()).ToList();



            return types;

        }



        public IReader GetReader(string extention)
        {
            var type = typeof(IReader);
            var readers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IReader)Activator.CreateInstance(c)).ToList();
            readers = readers.Where(p => p.GetExtention().Equals(extention)).ToList();

            if (readers.Count() < 0) return null;
            return readers[0];
        }

        public ISavable GetSaver(string extention)
        {
            var type = typeof(ISavable);
            var savers = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (ISavable)Activator.CreateInstance(c)).ToList();
            savers = savers.Where(p => p.GetExtention().Equals(extention)).ToList();

            if (savers.Count() < 0) return null;
            return savers[0];
        }

        public IConvertToDomain GetConvertToDomain(string extention)
        {
            var type = typeof(IConvertToDomain);
            var converters = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IConvertToDomain)Activator.CreateInstance(c)).ToList();
            converters = converters.Where(p => p.GetExtention().Equals(extention)).ToList();

            if (converters.Count() < 0) return null;
            return converters[0];
        }

        public IConvertToExtention GetConvertToExtention(string extention)
        {
            var type = typeof(IConvertToExtention);
            var converters = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IConvertToExtention)Activator.CreateInstance(c)).ToList();
            converters = converters.Where(p => p.GetExtention().Equals(extention)).ToList();

            if (converters.Count() < 0) return null;
            return converters[0];
        }
    }
}
