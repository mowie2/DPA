using DomainModel;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Savers
{
    public class SaveToLily : ISavable
    {   
        public void Save(string fileName, Symbol root)
        {
            DomainToLily domainToLily = new DomainToLily();
            
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                outputFile.Write(domainToLily.GetLilyText(root));
                outputFile.Close();
            }       
        }
    }
}
