using DomainModel;
using System.IO;

namespace LilypondAdapter
{
    public class SaveToLily : ISavable
    {
        public string extention { get; } = ".ly";
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
