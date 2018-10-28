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
        public readonly string extention = ".ly";
        public readonly string fancyName = "Lilypond";
        private IConvertToExtention converter;

        public SaveToLily()
        {
            converter = new DomainToLily();
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

        public string GetFancyName()
        {
            return fancyName;
        }
    }
}
