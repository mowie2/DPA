using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class LilyTokenizer
    {
        private readonly Dictionary<string, LilypondToken> lookupTable;
        private LilypondToken rootToken;

        public LilyTokenizer()
        {
            lookupTable = new Dictionary<string, LilypondToken>
            {
                ["\\relative"] = new LilypondToken { TokenKind = LilypondTokenKind.Relative },
                [@"([a - g])([eis] *)([, ']*)([0-14]+)([.]*)"] = new LilypondToken { TokenKind = LilypondTokenKind.Note },
                //Rest,
                //Bar,
                ["\\clef"] = new LilypondToken { TokenKind = LilypondTokenKind.Clef },
                ["treble"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["bass"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["alto"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["\\time"] = new LilypondToken { TokenKind = LilypondTokenKind.Time },
                ["\\tempo"] = new LilypondToken { TokenKind = LilypondTokenKind.Tempo },
                //[new Regex("\relative")] = new LilypondToken { TokenKind = LilypondTokenKind.Staff },
                ["\\repeat"] = new LilypondToken { TokenKind = LilypondTokenKind.Repeat },
                ["\\alternitive"] = new LilypondToken { TokenKind = LilypondTokenKind.Alternative },
                ["{"] = new LilypondToken { TokenKind = LilypondTokenKind.SectionStart },
                ["}"] = new LilypondToken { TokenKind = LilypondTokenKind.SectionEnd }
            };
        }
        public void ReadLily(string text)
        {
            string currentWord = "";
            int position = 0;
            LilypondToken prefToken = new LilypondToken();
            while (position <= text.Length)
            {
                if (text[position].Equals(" "))
                {
                    LilypondToken tempToken = new LilypondToken { TokenKind = LilypondTokenKind.Unknown };
                    if (lookupTable.ContainsKey(currentWord))
                    {
                        tempToken = lookupTable[currentWord];
                        tempToken.Value = currentWord;
                    }
                    if (rootToken == null)
                    {
                        rootToken = tempToken;
                    }
                    prefToken.NextToken = tempToken;
                    prefToken = tempToken;
                }
                else
                {
                    currentWord += text[position];
                }
                position++;
            }
        }
        public LilypondToken GetRootToken()
        {
            return rootToken;
        }
    }
}
