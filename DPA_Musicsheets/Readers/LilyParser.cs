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
        private Builders.NoteBuilder noteBuilder;
        private readonly Dictionary<string, Clef.Key> cleffs;
        private readonly Symbol[] symbols;
        private readonly Dictionary<string, Semitone.SEMITONE> pitchModifiers;

        public LilyParser()
        {
            noteBuilder = new Builders.NoteBuilder();
            cleffs = new Dictionary<string, Clef.Key>
            {
                ["treble"] = Clef.Key.G,
                ["bass"] = Clef.Key.F,
                ["alto"] = Clef.Key.C
            };
            pitchModifiers = new Dictionary<string, Semitone.SEMITONE>
            {
                ["es"] = Semitone.SEMITONE.MAJOR,
                ["is"] = Semitone.SEMITONE.MINOR,
                [""] = Semitone.SEMITONE.NORMAL
            };
            symbols = new Symbol[2];
        }

        public void ReadLily(LilypondToken rootToken)
        {
            LilypondToken currentToken = rootToken;
            while (currentToken.NextToken != null)
            {
                switch (currentToken.TokenKind)
                {
                    case LilypondTokenKind.Relative:
                        currentToken = FindRelative(currentToken);
                        break;
                    case LilypondTokenKind.Time:
                        currentToken = FindTimeSignature(currentToken);
                        break;
                    case LilypondTokenKind.Note:
                        Note newNote = FindNote(currentToken);
                        SetNextSymbol(symbols, newNote);
                        break;
                    case LilypondTokenKind.Rest:
                        Note newRest = FindRest(currentToken);
                        SetNextSymbol(symbols, newRest);
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

        private LilypondToken FindRelative(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.RelativeValue)
            {
                Regex re = new Regex(@"^c([,'])*$");
                var result = re.Match(currentToken.Value);
                noteBuilder.ModifyOctave(FindOctaveModifier(result.Groups[1].Value));
            }
            return currentToken;
        }


        private Symbol[] SetNextSymbol(Symbol[] symbols, Symbol nextSymbol)
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

        public Note FindRest(LilypondToken currentToken)
        {
            string text = currentToken.Value;
            Regex re = new Regex(@"^r(\d+)([.]*)$");
            Match result = re.Match(text);
            noteBuilder.SetDuriation(int.Parse(result.Groups[1].Value));
            noteBuilder.SetDotted(result.Groups[2].Value.Length);
            return noteBuilder.BuildNote();
        }

        private int FindOctaveModifier(string text)
        {
            int count = 0;
            foreach (char i in text)
            {
                if (i.Equals('\''))
                {
                    count += 1;
                }
                else if (i.Equals(','))
                {
                    count -= 1;
                }
            }
            return count;
        }

        private Note FindNote(LilypondToken currentToken)
        {
            string text = currentToken.Value;
            Regex re = new Regex(@"^([a-g])([eis]*)([,']*)(\d+)([.]*)$");

            Match result = re.Match(text);
            noteBuilder.SetPitch(result.Groups[1].Value);
            noteBuilder.SetSemitone(pitchModifiers[result.Groups[2].Value]);
            noteBuilder.ModifyOctave(FindOctaveModifier(result.Groups[3].Value));
            noteBuilder.SetDuriation(int.Parse(result.Groups[4].Value));
            noteBuilder.SetDotted(result.Groups[5].Value.Length);
            return noteBuilder.BuildNote();
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
            Clef tempClef = new Clef
            {
                key = cleffs[text]
            };
            noteBuilder.SetClef(tempClef);
        }

        private LilypondToken FindSection(LilypondToken startToken)
        {
            int countSectionStarts = 0;
            LilypondToken currentToken = startToken;

            while (currentToken.NextToken != null)
            {
                if (currentToken.TokenKind == LilypondTokenKind.SectionStart)
                {
                    countSectionStarts++;
                }
                else if (currentToken.TokenKind == LilypondTokenKind.SectionEnd)
                {
                    countSectionStarts--;
                }
                if(countSectionStarts == 0)
                {
                    break;
                }
                else if(countSectionStarts < 0)
                {
                    return startToken;
                }
                currentToken = currentToken.NextToken;
            }
            return currentToken;
        }

        private Note[] FindNoteSection(LilypondToken startToken,LilypondToken endToken)
        {
            LilypondToken currentToken = startToken;

            Note[] newSymbols = new Note[2];

            while ((currentToken != endToken && currentToken.NextToken != endToken) && currentToken.NextToken != null) 
            {
                currentToken = currentToken.NextToken;
                if (currentToken.TokenKind == LilypondTokenKind.Note)
                {
                    newSymbols = (Note[])SetNextSymbol(newSymbols, FindNote(currentToken));
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

        private LilypondToken FindTimeSignature(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.TimeValue)
            {
                SetTimeSignature(currentToken.Value);
            }
            return currentToken;

        }

        private void SetTimeSignature(string text)
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
