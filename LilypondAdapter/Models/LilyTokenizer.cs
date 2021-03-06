﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LilypondAdapter
{
    class LilyTokenizer
    {
        private readonly Dictionary<string, LilypondTokenKind> lookupTable;
        private LilypondToken rootToken;
        private LilypondToken prefToken;

        public LilyTokenizer()
        {
            lookupTable = new Dictionary<string, LilypondTokenKind>
            {
                [@"^\\relative$"] = LilypondTokenKind.Relative,
                [@"^c([,']*)$"] = LilypondTokenKind.RelativeValue,
                [@"^([a-g])([eis]*)([,']*)(\d+)([.]*)$"] = LilypondTokenKind.Note,
                [@"^r(\d+)([.]*)$"] = LilypondTokenKind.Rest,
                //Bar,
                [@"^\\clef$"] = LilypondTokenKind.Clef,
                [@"^treble$"] = LilypondTokenKind.ClefValue,
                [@"^bass$"] = LilypondTokenKind.ClefValue,
                [@"^alto$"] = LilypondTokenKind.ClefValue,
                [@"^\\time$"] = LilypondTokenKind.Time,
                [@"^(\d+)/(\d+)$"] = LilypondTokenKind.TimeValue,
                [@"^\\tempo$"] = LilypondTokenKind.Tempo,
                [@"^(\d+)=(\d+)$"] = LilypondTokenKind.TempoValue,
                [@"^\\repeat$"] = LilypondTokenKind.Repeat,
                [@"^\\alternative$"] = LilypondTokenKind.Alternative,
                [@"^{$"] = LilypondTokenKind.SectionStart,
                [@"^}$"] = LilypondTokenKind.SectionEnd 
            };
        }

        private void Clear()
        {
            rootToken = null;
            prefToken = null;
        }

        public void ReadLily(string text)
        {
            string currentWord = "";
            int position = 0;
            
            while (position < text.Length)
            {
                LilypondTokenKind[] resultToken = lookupTable.Where(pv => Regex.IsMatch(currentWord, pv.Key)).Select(pv => pv.Value).ToArray();
                char currentChar = text[position];
                if (currentChar.Equals(' '))
                {
                    LilypondToken tempToken = new LilypondToken { TokenKind = LilypondTokenKind.Unknown };
                    if (resultToken.Length > 0)
                    {
                        tempToken = new LilypondToken { TokenKind = resultToken[0] };
                        tempToken.Value = currentWord;
                    }
                    if (currentWord.Length > 0)
                    {
                        SetNextToken(tempToken);
                    }
                    currentWord = "";
                }
                else
                {
                    currentWord += currentChar;
                }
                position++;
            }
        }

        private void SetNextToken(LilypondToken nextToken)
        {
            if (rootToken == null)
            {
                rootToken = nextToken;
            }
            else
            {
                prefToken.NextToken = nextToken;
            }
            prefToken = nextToken;
        }

        public LilypondToken GetRootToken()
        {
            LilypondToken t = rootToken;
            rootToken = null;
            return t;
        }
    }
}
