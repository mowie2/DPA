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
        private string extention;

        public LillyPondReader()
        {
            extention = ".ly";
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
            root = ld.getRoot(file);
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
    }
}