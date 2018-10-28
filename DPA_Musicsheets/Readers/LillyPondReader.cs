using ClassLibrary;
using DPA_Musicsheets.Converters;
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
        private string liliePondText;
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
            root = ld.getRoot(file);
            return root;
        }


        private string ReadFile(string filePath)
        {
            liliePondText = System.IO.File.ReadAllText(filePath);
            return liliePondText;
        }
    }
}