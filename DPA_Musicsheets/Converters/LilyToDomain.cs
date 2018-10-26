using ClassLibrary;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converters
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
