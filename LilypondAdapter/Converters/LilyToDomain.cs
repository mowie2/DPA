using DomainModel;

namespace LilypondAdapter
{
    class LilyToDomain
    {
        public Symbol getRoot(string lilyText)
        {
            LilyTokenizer tokenizer = new LilyTokenizer();
            LilyParser parser = new LilyParser();
            string content = lilyText.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("}", " } ").Replace("{", " { ").Replace("  ", " ") + " ";
            tokenizer.ReadLily(content);
            parser.ReadLily(tokenizer.GetRootToken());
            Symbol root = parser.GetRootSymbol();
            return root;
        }
    }
}
