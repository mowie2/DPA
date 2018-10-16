using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class LilyTokenizer
    {
        private readonly Dictionary<string, LilypondToken> lookupTable;
        private LilypondToken rootToken;
        LilypondToken prefToken;

        public LilyTokenizer()
        {
            lookupTable = new Dictionary<string, LilypondToken>
            {
                ["\\relative"] = new LilypondToken { TokenKind = LilypondTokenKind.Relative },
                [@"([a-g])([eis]*)([,']*)([0-14]+)([.]*)"] = new LilypondToken { TokenKind = LilypondTokenKind.Note },
                [@"(r)(\d+)"] = new LilypondToken { TokenKind = LilypondTokenKind.Rest },
                //Bar,
                ["\\clef"] = new LilypondToken { TokenKind = LilypondTokenKind.Clef },
                ["treble"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["bass"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["alto"] = new LilypondToken { TokenKind = LilypondTokenKind.ClefValue },
                ["\\time"] = new LilypondToken { TokenKind = LilypondTokenKind.Time },
                [@"(\d+)/(\d+)"] = new LilypondToken { TokenKind = LilypondTokenKind.TimeValue},
                ["\\tempo"] = new LilypondToken { TokenKind = LilypondTokenKind.Tempo },
                ["\\repeat"] = new LilypondToken { TokenKind = LilypondTokenKind.Repeat },
                ["\\alternitive"] = new LilypondToken { TokenKind = LilypondTokenKind.Alternative },
                ["{"] = new LilypondToken { TokenKind = LilypondTokenKind.SectionStart },
                ["}"] = new LilypondToken { TokenKind = LilypondTokenKind.SectionEnd }
            };
            rootToken = new LilypondToken();
            prefToken = new LilypondToken();
        }
        public void ReadLily(string text)
        {
            string currentWord = "";
            int position = 0;
            
            while (position <= text.Length)
            {
                LilypondToken[] resultToken = lookupTable.Where(pv => Regex.IsMatch(currentWord, pv.Key)).Select(pv => pv.Value).ToArray();
                if (text[position].Equals(" ") || resultToken.Length > 0)
                {
                    LilypondToken tempToken = new LilypondToken { TokenKind = LilypondTokenKind.Unknown };
                    if (resultToken.Length > 0)
                    {
                        tempToken = resultToken[0];
                        tempToken.Value = currentWord;
                    }
                    SetNextToken(tempToken);
                    currentWord = "";
                }
                else
                {
                    currentWord += text[position];
                }
                position++;
            }
        }

        public void SetNextToken(LilypondToken nextToken)
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
            return rootToken;
        }
    }
}
