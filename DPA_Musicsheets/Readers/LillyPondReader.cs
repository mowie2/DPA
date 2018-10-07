using ClassLibrary;
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
        
        private readonly string[] pitches = new string[] { "c", "d", "e", "f", "g", "a", "b" };
        private readonly string[] keyWords = new string[] { "\\relative", "\\clef", "\\time", "\\repeat", "\\alternitive" ,"Volta"};
        private readonly string[] clefs = new string[] { "treble", "bass", "alto" };
        private readonly string[] breakers = new string[] { "{", "}"};

        public void ReadLilly(string text)
        {
            LilyParser parser = new LilyParser();
            string copyText = text.ToLower();
            string[] splitText = copyText.Split(' ');
            int position = 0;

            while (true)
            {
                if (position + 1 == splitText.Length) { break; }
                string currentText = splitText[position];
                if (keyWords.Contains(currentText))
                {
                    Regex re;
                    switch (currentText)
                    {
                        case "relative":
                            //geen idee wat dit moet doen????????????????????????????????????????????
                            break;
                        case "clef":
                            position += 1;
                            currentText = splitText[position];
                            if (clefs.Contains(currentText))
                            {
                                parser.FindClef(currentText);
                            }
                            break;
                        case "time":
                            position += 1;
                            currentText = splitText[position];
                            re = new Regex(@"(\d+)/(\d+)");
                            if (re.IsMatch(currentText))
                            {
                                Match result = re.Match(currentText);
                                int numberOfBeats = int.Parse(result.Groups[1].Value);
                                int timeOfBeats = int.Parse(result.Groups[2].Value);
                                parser.FindTimeSignature(numberOfBeats,timeOfBeats);
                            }
                            break;
                        case "repeat":
                            break;
                        case "alternitive":
                            break;
                        default:
                            re = new Regex(@"([a-g])([eis]*)([,']*)([0-14]+)([.]*)");
                            if (re.IsMatch(currentText))
                            {
                                Match result = re.Match(currentText);
                                string pitch = result.Groups[1].Value;
                                string pitchModifier = result.Groups[2].Value;
                                string octaveModifier = result.Groups[3].Value;
                                int duration = int.Parse(result.Groups[4].Value);
                                int dotted = result.Groups[5].Value.Length;
                                parser.FindNote(pitch, pitchModifier, octaveModifier, duration, dotted);
                                parser.getNote();
                            }
                            break;
                    }
                    position += 1;
                }
            }
        }
    }
}