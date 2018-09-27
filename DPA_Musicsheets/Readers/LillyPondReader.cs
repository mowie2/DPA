using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Readers
{
    class LillyPondReader
    {
        private Dictionary<string, Clef> clefDictionary = new Dictionary<string, Clef>();
        private string[] pitches = new string[] { "c", "d", "e", "f", "g", "a", "b" };
        private string[] keyWords = new string[] { "relative", "clef", "time", "repeat", "alternitive" };
        private string[] breakers = new string[] { "{", "}"," " };
        //private string[] uselessWords = new string[] { "clef" };
        //keywords??????

        private Builders.NoteBuilder noteBuilder = new Builders.NoteBuilder();

        public LillyPondReader()
        {
            Clef tempClef = new Clef();
            tempClef.key = Clef.Key.G;
            clefDictionary.Add("treble", tempClef);
            tempClef.key = Clef.Key.F;
            clefDictionary.Add("bass", tempClef);
            tempClef.key = Clef.Key.C;
            clefDictionary.Add("alto", tempClef);
        }

        public void readLilly(string text)
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
                            if (clefDictionary.ContainsKey(currentText))
                            {
                                noteBuilder.SetClef(clefDictionary[currentText]);
                            }
                            break;
                        case "time":
                            position += 1;
                            currentText = splitText[position];
                            if (currentText.Contains("/"))
                            {
                                string[] tempString = currentText.Split('/');
                                bool isNumber0 = int.TryParse(tempString[0], out int numberOfBeats);
                                bool isNumber1 = int.TryParse(tempString[1], out int timeOfBeats);
                                if (isNumber0 && isNumber1)
                                {
                                    TimeSignature ts = new TimeSignature();
                                    ///Geen idee of dit goed is????????????????????????????????????????????????????????
                                    ts.NumberOfBeats = numberOfBeats;
                                    ts.TimeOfBeats = timeOfBeats;
                                    noteBuilder.SetTimeSignature(ts);
                                }
                            }
                            break;
                        case "alternitive":
                            break;
                        default:
                            //build note/////////////////////////////////////////////////////////////////////////////////
                            //aparte functie??????????????????
                            foreach (char i in currentText)
                            {
                                int checkmarks = 0;
                                if (pitches.Contains(i.ToString()))
                                {
                                    checkmarks += 1;
                                    noteBuilder.SetPitch(i.ToString());
                                }
                            }
                            break;
                    }
                    position += 1;
                }


                /*
                while (true)
                {
                    if (position + 1 == splitText.Length) { break; }
                    if (!uselessWords.Contains(splitText[position]))
                    {
                        if (splitText[position].Equals("relative"))
                        {
                            position += 1;
                            //relative (c') ????????????
                        }
                        else if (splitText[position].Equals("time"))
                        {
                            position += 1;
                            //time (4/4)' ????????????
                        }
                        else if (clefDictionary.ContainsKey(splitText[position]))
                        {
                            //set clef
                            noteBuilder.SetClef(clefDictionary[splitText[position]]);
                        }
                        position += 1;
                    }
                    else
                    {
                        //note g'4...??????
                    }
                */
            }
        }
    }
}