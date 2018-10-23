using ClassLibrary;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Readers
{
    public class LillyPondReader : IReader
    {
        private LilyParser parser;
        private LilyTokenizer tokenizer; 
        private Symbol root;
        public LillyPondReader()
        {
            parser = new LilyParser();
            tokenizer = new LilyTokenizer();
        }

        public Symbol readFile(string filename)
        {
            tokenizer.ReadLily(filename);
            parser.ReadLily(tokenizer.GetRootToken());
            root = parser.GetRootSymbol();

            return root;
        }

    }
}