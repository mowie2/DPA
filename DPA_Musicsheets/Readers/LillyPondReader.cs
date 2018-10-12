using ClassLibrary;
using DPA_Musicsheets.Models;
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
        //private readonly string[] keyWords = new string[] { "\\relative", "\\clef", "\\time", "\\repeat", "\\alternitive" ,"Volta"};
        //private readonly string[] clefs = new string[] { "treble", "bass", "alto" };
        //private readonly string startSection = "{";
        //private readonly string endSection = "}";

        private LilyParser parser = new LilyParser();
        private LilyTokenizer tokenizer = new LilyTokenizer(); 
        private Note root;
        private Note prefNote;


        public void ReadLily(string text)
        {
            tokenizer.ReadLily(text);
            tokenizer.GetRootToken();
        }




        /*
        public void FindClef(string text)
        {
            if (clefs.Contains(text))
            {
                parser.FindClef(text);
            }
        }

        public void FindTime(string text)
        {
            Regex re = new Regex(@"(\d+)/(\d+)");
            if (re.IsMatch(text))
            {
                Match result = re.Match(text);
                int numberOfBeats = int.Parse(result.Groups[1].Value);
                int timeOfBeats = int.Parse(result.Groups[2].Value);
                parser.FindTimeSignature(numberOfBeats, timeOfBeats);
            }
        }
        
        public bool FindNote(string text)
        {
            Regex re = new Regex(@"([a-g])([eis]*)([,']*)([0-14]+)([.]*)");
            bool returner = re.IsMatch(text);
            if (returner)
            {
                Match result = re.Match(text);
                string pitch = result.Groups[1].Value;
                string pitchModifier = result.Groups[2].Value;
                string octaveModifier = result.Groups[3].Value;
                int duration = int.Parse(result.Groups[4].Value);
                int dotted = result.Groups[5].Value.Length;
                parser.FindNote(pitch, pitchModifier, octaveModifier, duration, dotted);
                Note tempNote = parser.GetNote();
                if (root == null)
                {
                    root = tempNote;
                }
                prefNote.NextNote = tempNote;
                prefNote = tempNote;
                
            }
            return returner;
        }

        public void FindRepeat(string text)
        {
            string[] splitText = text.Split(';');
            foreach (string s in splitText)
            {
                FindNote(s);
            }
        }

        public void FindSection(string[] text,int position)
        {

            string currentText = text[position];
            if (currentText.Equals(startSection))
            {
                position += 1;
                string tempText = text[position];
                while (position <= text.Length || tempText.Equals(endSection))
                {
                    position += 1;
                    tempText = text[position] + ";";
                    currentText += tempText;
                }
            }
        }
        */


        /*
        public void ReadLilly(string text)
        {
            string copyText = text.ToLower();
            string[] splitText = copyText.Split(' ');
            int position = 0;

            while (true)
            {
                if (position + 1 == splitText.Length) { break; }
                string currentText = splitText[position];
                if (keyWords.Contains(currentText))
                {
                    switch (currentText)
                    {
                        case "relative":
                            //geen idee wat dit moet doen????????????????????????????????????????????
                            break;
                        case "clef":
                            position += 1;
                            currentText = splitText[position];
                            FindClef(currentText);
                            break;
                        case "time":
                            position += 1;
                            currentText = splitText[position];
                            FindTime(currentText);
                            break;
                        case "repeat":
                            position += 1;
                            currentText = splitText[position];
                            if (currentText.Equals(startSection))
                            {
                                string tempText = splitText[position];
                                while (position <= text.Length || tempText.Equals(endSection))
                                {
                                    position += 1;
                                    tempText = splitText[position]+";";
                                    currentText += tempText;
                                }
                            }
                            FindRepeat(currentText);
                            break;
                        case "alternitive":
                            position += 1;
                            
                            break;
                        default:
                            if (FindNote(currentText))
                            {
                                Note newNote = parser.GetNote();
                            }
                            break;
                    }
                    position += 1;
                }
            }
        }
        */
    }
}