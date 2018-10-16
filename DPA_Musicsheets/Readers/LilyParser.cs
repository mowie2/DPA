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
        private Symbol rootSymbol;
        private Symbol prefSymbol;

        public void ReadLily(LilypondToken rootToken)
        {
            LilypondToken currentToken = rootToken;
            while(currentToken.NextToken != null)
            {
                switch (currentToken.TokenKind)
                {
                    case LilypondTokenKind.Relative:
                        break;
                    case LilypondTokenKind.Time:
                        currentToken = FindTimeSignature(currentToken);
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
                    case LilypondTokenKind.Alternative:
                        currentToken = currentToken.NextToken;
                        currentToken = FindRepeat(currentToken);
                        break;
                }
                currentToken = currentToken.NextToken;
            }
        }

        private void SetNextSymbol(Symbol nextSymbol)
        {
            if(rootSymbol == null)
            {
                rootSymbol = nextSymbol;
            }
            prefSymbol.nextSymbol = nextSymbol;
            prefSymbol = nextSymbol;
        }

        public void SetNote(string text)
        {
            Regex re = new Regex(@"([a-g])([eis]*)([,']+*)([0-14]+)([.]+*)");
            if (re.IsMatch(text))
            {
                Match result = re.Match(text);
                noteBuilder.SetPitch(result.Groups[1].Value);
                switch (result.Groups[2].Value)
                {
                    case "es":
                        noteBuilder.SetSemitone(Semitone.SEMITONE.MAJOR);
                        break;
                    case "is":
                        noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                        break;
                }
                foreach (char i in result.Groups[3].Value)
                {
                    if (i.Equals('\''))
                    {
                        noteBuilder.ModifyOctave(1);
                    }
                    if (i.Equals(','))
                    {
                        noteBuilder.ModifyOctave(-1);
                    }
                }
                noteBuilder.SetDuriation(int.Parse(result.Groups[4].Value));
                noteBuilder.SetDotted(result.Groups[5].Value.Length);
                SetNextSymbol(noteBuilder.BuildNote());
            }
        }

        private LilypondToken FindCef(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if (currentToken.TokenKind == LilypondTokenKind.ClefValue)
            {
                SetClef(currentToken.Value);
            }
            return currentToken;
        }

        private void SetClef(string text)
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

        private LilypondToken FindSection(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken;
            if (currentToken.TokenKind == LilypondTokenKind.SectionStart)
            {
                currentToken = currentToken.NextToken;
                while (currentToken.NextToken.TokenKind != LilypondTokenKind.SectionEnd)
                {
                    if (currentToken.NextToken == null)
                    {
                        return null;
                    }
                    else
                    {
                        currentToken = currentToken.NextToken;
                        if (currentToken.TokenKind != LilypondTokenKind.Note)
                        {
                            currentToken = currentToken.NextToken;
                            continue;
                        }
                        SetNote(currentToken.Value);
                    }
                }
            }
            return currentToken;
        }


        private LilypondToken FindRepeat(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken.NextToken.NextToken;
            BarLine firstBarline = new BarLine { Type = BarLine.TYPE.REPEAT };
            BarLine lastBarline = new BarLine
            {
                Type = BarLine.TYPE.REPEAT,
                Buddy = firstBarline
            };
            LilypondToken lastToken = FindSection(currentToken);
            if (lastToken != null)
            {
                while (currentToken != lastToken)
                {
                    currentToken = currentToken.NextToken;
                    if(currentToken.TokenKind == LilypondTokenKind.Note)
                    {
                        SetNote(currentToken.Value);
                    }
                }
            }
            return currentToken;
        }

        private LilypondToken FindAlternitive(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken.NextToken.NextToken;
            BarLine firstBarline = new BarLine { Type = BarLine.TYPE.REPEAT };
            BarLine lastBarline = new BarLine
            {
                Type = BarLine.TYPE.REPEAT,
                Buddy = firstBarline
            };
            SetNextSymbol(firstBarline);
            if (currentToken.TokenKind == LilypondTokenKind.SectionStart)
            {
                currentToken = currentToken.NextToken;
                while (currentToken.NextToken.TokenKind != LilypondTokenKind.SectionEnd
                    && currentToken.NextToken != null)
                {
                    currentToken = currentToken.NextToken;
                    if (currentToken.TokenKind != LilypondTokenKind.Note)
                    {
                        currentToken = currentToken.NextToken;
                        continue;
                    }
                    SetNote(currentToken.Value);
                }
                SetNextSymbol(lastBarline);
            }
            return currentToken;
        }


        public LilypondToken FindTimeSignature(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.TimeValue)
            {
                SetTimeSignature(currentToken.Value);
            }
            return currentToken;

        }

        public void SetTimeSignature(string text)
        {
            TimeSignature t = new TimeSignature();
            Regex re = new Regex(@"(\d+)/(\d+)");
            int NumberOfBeats;
            int TimeOfBeats;
            if (re.IsMatch(text))
            {
                var result = re.Match(text);
                NumberOfBeats = int.Parse(result.Groups[1].Value);
                TimeOfBeats = int.Parse(result.Groups[2].Value);
                noteBuilder.SetTimeSignature(t);
            };
        }



        public Symbol GetRootSymbol()
        {
            return rootSymbol;
        }
    }
}
