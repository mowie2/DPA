﻿using DomainModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LilypondAdapter
{
    class LilyParser
    {
        private NoteBuilder noteBuilder;
        private Dictionary<string, Clef.Key> cleffs;
        private Dictionary<char, int> octaveModifier;
        private Symbol[] symbols;
        private Dictionary<string, Semitone.SEMITONE> pitchModifiers;
        Dictionary<LilypondTokenKind, Delegate> parserFunctions;
        List<string> notesOrder = new List<string>() { "c", "d", "e", "f", "g", "a", "b" };
        private string prefPitch;

        public void ReadLily(LilypondToken rootToken)
        {
            Clear();
            LilypondToken currentToken = rootToken;
            while (currentToken != null)
            {
                if (parserFunctions.ContainsKey(currentToken.TokenKind))
                {
                    currentToken = (LilypondToken)parserFunctions[currentToken.TokenKind].DynamicInvoke(currentToken);
                }
                currentToken = currentToken.NextToken;
            }
        }

        private void Clear()
        {
            symbols = new Symbol[2];
            prefPitch = "";
            noteBuilder = new NoteBuilder();
            cleffs = new Dictionary<string, Clef.Key>
            {
                ["treble"] = Clef.Key.G,
                ["bass"] = Clef.Key.F,
                ["alto"] = Clef.Key.C
            };
            pitchModifiers = new Dictionary<string, Semitone.SEMITONE>
            {
                ["s"] = Semitone.SEMITONE.MAJOR,
                ["es"] = Semitone.SEMITONE.MAJOR,
                ["is"] = Semitone.SEMITONE.MINOR,
                [""] = Semitone.SEMITONE.NORMAL
            };
            octaveModifier = new Dictionary<char, int>
            {
                ['\''] = 1,
                [','] = -1
            };
            
            parserFunctions = new Dictionary<LilypondTokenKind, Delegate>
            {
                [LilypondTokenKind.Relative] = new Func<LilypondToken, LilypondToken>(FindRelative),
                [LilypondTokenKind.Time] = new Func<LilypondToken, LilypondToken>(FindTimeSignature),
                [LilypondTokenKind.Tempo] = new Func<LilypondToken, LilypondToken>(FindTempo),
                [LilypondTokenKind.Note] = new Func<LilypondToken, LilypondToken>(SetNextNote),
                [LilypondTokenKind.Rest] = new Func<LilypondToken, LilypondToken>(SetNextRest),
                [LilypondTokenKind.Clef] = new Func<LilypondToken, LilypondToken>(FindClef),
                [LilypondTokenKind.Repeat] = new Func<LilypondToken, LilypondToken>(SetRepeat),
                [LilypondTokenKind.Alternative] = new Func<LilypondToken, LilypondToken>(SetAlternitive),
            };
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

        private int RelativeOctaveModifier(string pitch)
        {
            int returnOctave = 0;
            if (!prefPitch.Equals(""))
            {
                int distance = notesOrder.IndexOf(pitch) - notesOrder.IndexOf(prefPitch);
                if (distance > 3)
                {
                    returnOctave -= 1;
                }
                else if (distance < -3)
                {
                    returnOctave += 1;
                }
            }
            prefPitch = pitch;
            return returnOctave;
        }

        private int FindOctaveModifier(string text)
        {
            int count = 0;
            foreach (char i in text)
            {
                count += octaveModifier[i];
            }
            return count;
        }

        private LilypondToken FindRelative(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.RelativeValue)
            {
                Regex re = new Regex(@"^c([,']*)$");
                var result = re.Match(currentToken.Value);
                noteBuilder.ModifyOctave(FindOctaveModifier(result.Groups[1].Value));
            }
            return currentToken;
        }

        private LilypondToken FindClef(LilypondToken startToken)
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

        private LilypondToken FindTempo(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if(currentToken.TokenKind == LilypondTokenKind.TempoValue)
            {
                SetTempo(currentToken.Value);    
            }
            return currentToken;
        }

        private void SetTempo(string text)
        {
            Tempo newTempo = new Tempo();
            var result = Regex.Match(text, @"^(\d+)=(\d+)$");
            newTempo.noteDuration = int.Parse(result.Groups[1].Value);
            newTempo.bpm = int.Parse(result.Groups[2].Value);
            noteBuilder.SetTempo(newTempo);
        }

        private LilypondToken FindTimeSignature(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken;
            if (currentToken.TokenKind == LilypondTokenKind.TimeValue)
            {
                SetTimeSignature(currentToken.Value);
            }
            return currentToken;

        }

        private void SetTimeSignature(string text)
        {
            Regex re = new Regex(@"(\d+)/(\d+)");
            var result = re.Match(text);
            noteBuilder.SetTimeSignature(new TimeSignature
            {
                NumberOfBeats = int.Parse(result.Groups[1].Value),
                TimeOfBeats = int.Parse(result.Groups[2].Value)
            });
        }

        private Note[] FindNoteSection(LilypondToken startToken, LilypondToken endToken)
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
                if (countSectionStarts == 0)
                {
                    break;
                }
                else if (countSectionStarts < 0)
                {
                    return startToken;
                }
                currentToken = currentToken.NextToken;
            }
            return currentToken;
        }

        private Note FindNote(LilypondToken currentToken)
        {
            string text = currentToken.Value;
            Regex re = new Regex(@"^([a-g])([eis]*)([,']*)(\d+)([.]*)$");

            Match result = re.Match(text);
            noteBuilder.SetPitch(result.Groups[1].Value);
            noteBuilder.SetSemitone(pitchModifiers[result.Groups[2].Value]);
            noteBuilder.ModifyOctave(FindOctaveModifier(result.Groups[3].Value)+RelativeOctaveModifier(result.Groups[1].Value));
            noteBuilder.SetDuriation(int.Parse(result.Groups[4].Value));
            noteBuilder.SetDotted(result.Groups[5].Value.Length);
            return noteBuilder.BuildNote();
        }

        private LilypondToken SetNextNote(LilypondToken startToken)
        {
            Note n = FindNote(startToken);
            SetNextSymbol(symbols, n);
            return startToken;
        }

        private Note FindRest(LilypondToken currentToken)
        {
            string text = currentToken.Value;
            Regex re = new Regex(@"^r(\d+)([.]*)$");
            Match result = re.Match(text);
            noteBuilder.SetDuriation(int.Parse(result.Groups[1].Value));
            noteBuilder.SetDotted(result.Groups[2].Value.Length);
            return noteBuilder.BuildNote();
        }

        private LilypondToken SetNextRest(LilypondToken startToken)
        {
            Note n = FindRest(startToken);
            SetNextSymbol(symbols, n);
            return startToken;
        }

        private LilypondToken SetRepeat(LilypondToken startToken)
        {
            LilypondToken currentToken = startToken.NextToken.NextToken.NextToken;
            BarLine firstBarline = new BarLine { Type = BarLine.TYPE.START };
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
        
        public Symbol GetRootSymbol()
        {
            Symbol s = symbols[0];
            symbols[0] = null;
            symbols[1] = null;
            return s;
        }
    }
}
