using DomainModel;
using DPA_Musicsheets.Interfaces;

namespace LilypondAdapter
{
    public class LillyPondReader : IReader
    {
        private LilyParser parser;
        private LilyTokenizer tokenizer; 
        private Symbol root;
        private string liliePondText;
        private readonly string extention = ".ly";
        private readonly string fancyName = "Lilypond";

        public LillyPondReader()
        {
            parser = new LilyParser();
            tokenizer = new LilyTokenizer();
        }

        public string GetMusicText()
        {
            return liliePondText;
        }

        public Symbol readFile(string filename)
        {
            string file = ReadFile(filename);
            LilyToDomain ld = new LilyToDomain();
            root = ld.Convert(file);
            //SaveToLily s = new SaveToLily();
            //s.Save("test.ly", root);
            
            
            return root;
        }

        
        private string ReadFile(string filePath)
        {
            liliePondText = System.IO.File.ReadAllText(filePath);
            return liliePondText;
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