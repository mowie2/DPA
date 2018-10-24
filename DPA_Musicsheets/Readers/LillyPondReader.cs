using ClassLibrary;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.Savers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Readers
{
    class LillyPondReader
    {
        private LilyParser parser;
        private LilyTokenizer tokenizer; 
        private Symbol root;
        public LillyPondReader()
        {
            parser = new LilyParser();
            tokenizer = new LilyTokenizer();
        }


        public void ReadLily(string text)
        {
            string content = text.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("  "," ")+" ";
            tokenizer.ReadLily(content);
            parser.ReadLily(tokenizer.GetRootToken());
            root = parser.GetRootSymbol();
            SaveToLily s = new SaveToLily();
            s.Save("jifljlf.ly",root);
        }
    }
}