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
        private Symbol[] symbols;

        public LilyParser()
        {
            symbols = new Symbol[2];
        }

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
                        Note newNote = FindNote(currentToken.Value);
                        SetNextSymbol(symbols,newNote);
                        break;
                    case LilypondTokenKind.Clef:
                        currentToken = FindCef(currentToken);
                        break;
                    case LilypondTokenKind.Repeat:
                        currentToken = SetRepeat(currentToken);
                        break;
                    case LilypondTokenKind.Alternative:
                        currentToken = SetAlternitive(currentToken);
                        break;
                }
                currentToken = currentToken.NextToken;
            }
        }

        private Symbol[] SetNextSymbol(Symbol[] symbols,Symbol nextSymbol)
        {
            Symbol rootSymbol = symbols[0];
            if (rootSymbol == null)
            {
                symbols[0] = nextSymbol;
            }
            else
            {
                symbols[1].nextSymbol = nextSymbol;
            }
            symbols[1] = nextSymbol;
            return symbols;
        }

        public Note FindNote(string text)
        {
            Regex re = new Regex(@"([a-g])([eis]*)([,']*)([0-9]+)([.]*)");
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
                return noteBuilder.BuildNote();
            }
            return null;
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
            int countSectionStarts = 0;
            LilypondToken currentToken = startToken;
            if (currentToken.TokenKind == LilypondTokenKind.SectionStart)
            {
                countSectionStarts++;
                while (currentToken.NextToken != null && countSectionStarts!=0)
                {
                    currentToken = currentToken.NextToken;
                    if (currentToken.TokenKind == LilypondTokenKind.SectionStart)
                    {
                        countSectionStarts++;
                    }
                    else if(currentToken.TokenKind == LilypondTokenKind.SectionEnd)
                    {
                        countSectionStarts--;
                    }          
                }
            }
            return currentToken;
        }

        public Note[] FindNoteSection(LilypondToken startToken,LilypondToken endToken)
        {
            LilypondToken currentToken = startToken;

            Note[] newSymbols = new Note[2];

            while ((currentToken != endToken && currentToken.NextToken != endToken) && currentToken.NextToken != null) 
            {
                currentToken = currentToken.NextToken;
                if (currentToken.TokenKind == LilypondTokenKind.Note)
                {
                    newSymbols = (Note[])SetNextSymbol(newSymbols, FindNote(currentToken.Value));
                }
            }
            return newSymbols;
        }

        private LilypondToken SetRepeat(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken.NextToken.NextToken;
            BarLine firstBarline = new BarLine { Type = BarLine.TYPE.REPEAT };
            BarLine lastBarline = new BarLine
            {
                Type = BarLine.TYPE.REPEAT,
                Buddy = firstBarline
            };
            LilypondToken lastToken = FindSection(currentToken);
            SetNextSymbol(symbols,firstBarline);
            Symbol[] newSymbols = FindNoteSection(currentToken, lastToken);
            SetNextSymbol(symbols,newSymbols[0]);
            symbols[1] = newSymbols[1];
            SetNextSymbol(symbols,lastBarline);
            return lastToken;
        }

        private LilypondToken SetAlternitive(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            LilypondToken lastToken = FindSection(currentToken);
            Symbol prevSymbol = symbols[1];

            if (prevSymbol.GetType() == typeof(BarLine))
            {
                BarLine barline = prevSymbol as BarLine;
                while (currentToken != lastToken && currentToken.NextToken != lastToken)
                {
                    currentToken = currentToken.NextToken;
                    LilypondToken templastToken = FindSection(currentToken);
                    Note tempRoot = FindNoteSection(currentToken, templastToken)[0];
                    currentToken = templastToken;
                    barline.Alternatives.Add(tempRoot);
                }
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
            Regex re = new Regex(@"(\d+)/(\d+)");
            if (re.IsMatch(text))
            {
                var result = re.Match(text);
                noteBuilder.SetTimeSignature(new TimeSignature
                {
                    NumberOfBeats = int.Parse(result.Groups[1].Value),
                    TimeOfBeats = int.Parse(result.Groups[2].Value)
                });
            };
        }

        public Symbol GetRootSymbol()
        {
            return symbols[0];
        }
    }
}
