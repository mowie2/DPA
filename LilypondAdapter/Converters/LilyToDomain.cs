using ClassLibrary.Interfaces;
using DomainModel;

namespace LilypondAdapter
{
    public class LilyToDomain : IConvertToDomain
    {
        private string extention;
        public LilyToDomain()
        {
            extention = ".ly";
        }

        public Symbol Convert(object text)
        {
            string lilyText = text as string;
            LilyTokenizer tokenizer = new LilyTokenizer();
            LilyParser parser = new LilyParser();
            string content = lilyText.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("}", " } ").Replace("{", " { ").Replace("  ", " ") + " ";
            tokenizer.ReadLily(content);
            parser.ReadLily(tokenizer.GetRootToken());
            Symbol root = parser.GetRootSymbol();
            return root;
        }

        public string GetExtention()
        {
            return extention;
        }
    }
}
