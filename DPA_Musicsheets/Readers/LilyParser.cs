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
    class LilyParser
    {
        private Builders.NoteBuilder noteBuilder = new Builders.NoteBuilder();

        public void ReadLily(LilypondToken rootToken)
        {
            LilypondToken currentToken = rootToken;
            while(currentToken.NextToken != null)
            {
                switch (currentToken.TokenKind)
                {
                    case LilypondTokenKind.Alternative:
                        break;
                    case LilypondTokenKind.Note:
                        SetNote(currentToken.Value);
                        break;
                    case LilypondTokenKind.Clef:
                        currentToken = currentToken.NextToken;
                        currentToken = FindCef(currentToken);
                        break;
                    case LilypondTokenKind.Repeat:
                        currentToken = currentToken.NextToken;
                        currentToken = FindRepeat(currentToken);
                        break;
                }
                currentToken = currentToken.NextToken;
            }
        }

        public void SetNote(string text)
        {
            Regex re = new Regex(@"([a-g])([eis]*)([,']*)([0-14]+)([.]*)");
            if (re.IsMatch(text))
            {
                Match result = re.Match(text);
                string pitch = result.Groups[1].Value;
                string pitchModifier = result.Groups[2].Value;
                string octaveModifier = result.Groups[3].Value;
                int duration = int.Parse(result.Groups[4].Value);
                int dotted = result.Groups[5].Value.Length;
            }
        }

        public LilypondToken FindCef(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if (currentToken.TokenKind == LilypondTokenKind.ClefValue)
            {
                SetClef(currentToken.Value);
            }
            return currentToken;
        }

        public void SetClef(string text)
        {
            Clef tempClef = new Clef();
            switch (text)
            {
                case "treble":
                    tempClef.key = Clef.Key.G;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "bass":
                    tempClef.key = Clef.Key.F;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "alto":
                    tempClef.key = Clef.Key.C;
                    noteBuilder.SetClef(tempClef);
                    break;
            }
        }

        public LilypondToken FindRepeat(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.SectionStart)
            {
                string currentText = "";
                currentToken = currentToken.NextToken;
                while(currentToken.TokenKind != LilypondTokenKind.SectionEnd)
                {
                    if(currentToken.NextToken == null)
                    {
                        break;
                    }
                    currentText += currentToken.Value;
                    currentToken = currentToken.NextToken;
                }
                SetClef(currentText);
            }
            return currentToken;
        }

        public void SetRepeat(string text)
        {

        }






        /*
        public void FindClef(string text)
        {
            Clef tempClef = new Clef();
            switch (text)
            {
                case "treble":
                    tempClef.key = Clef.Key.G;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "bass":
                    tempClef.key = Clef.Key.F;
                    noteBuilder.SetClef(tempClef);
                    break;
                case "alto":
                    tempClef.key = Clef.Key.C;
                    noteBuilder.SetClef(tempClef);
                    break;
            }
        }
        */
        public void FindTimeSignature(int numberOfBeats,int timeOfBeats)
        {
            TimeSignature t = new TimeSignature
            {
                NumberOfBeats = numberOfBeats,
                TimeOfBeats = timeOfBeats
            };
            noteBuilder.SetTimeSignature(t);
        }
        public void FindNote(string pitch,string pitchModifier,string octaveModifier,int duration,int dotted)
        {
            noteBuilder.SetPitch(pitch);
            //noteBuilder.setPitchModifier(pitchModifier);
            //noteBuilder.setOctaveModifier(octaveModifier);
            noteBuilder.SetDuriation((float)duration);
            noteBuilder.SetDotted(dotted);
        }
        
        

        public Note GetRootNote()
        {
            return noteBuilder.BuildNote();
        }
    }
}
