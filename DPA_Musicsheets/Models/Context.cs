using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Context
    {
        private Builders.NoteBuilder noteBuilder;
        private readonly Dictionary<string, Clef.Key> cleffs;
        private readonly Dictionary<char, int> octaveModifier;
        private readonly Symbol[] symbols;
        private readonly Dictionary<string, Semitone.SEMITONE> pitchModifiers;
        Dictionary<LilypondTokenKind, Delegate> parserFunctions;
        private readonly string[] pitches = { "c", "d", "e", "f", "g", "a", "b", };
        private string relativePitch;
        bool relative;

        public Context()
        {

        }
    }
}
