using ClassLibrary;
using DPA_Musicsheets.Interfaces;
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
            string file = ReadFile(filename);
            string content = file.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("  "," ")+" ";
            tokenizer.ReadLily(content);
            parser.ReadLily(tokenizer.GetRootToken());
            root = parser.GetRootSymbol();
            //SaveToMidi d = new SaveToMidi();
            //d.Save("newtest.mid", root);
            SaveToLily s = new SaveToLily();
            s.Save("tfewflwlj.ly",root);
            return root;
        }


        private string ReadFile(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }
    }
}