using ClassLibrary;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Facade
{
    public class PsamContolLib
    {
        private List<MusicalSymbol> symbols;

        private Symbol currentNote;
        private Dictionary<Type, Func<MusicalSymbol>> actions;
        private readonly Dictionary<ClassLibrary.Clef.Key, ClefType> clefs;
        private readonly ClassLibrary.TimeSignature timeSignature;
        private readonly ClassLibrary.Clef clef;
        private float barlinecount = 0;
        public PsamContolLib()
        {
            actions = new Dictionary<Type, Func<MusicalSymbol>>
            {
                { typeof(ClassLibrary.Note), DoNote },
            };

            clefs = new Dictionary<ClassLibrary.Clef.Key, ClefType>
            {
                { ClassLibrary.Clef.Key.C, ClefType.CClef },
                { ClassLibrary.Clef.Key.F, ClefType.FClef },
                { ClassLibrary.Clef.Key.G, ClefType.GClef }
            };
            clef = new ClassLibrary.Clef();
            
            timeSignature = new ClassLibrary.TimeSignature();


        }

        public IList<MusicalSymbol> GetStaffsFromTokens(Symbol rootNote)
        {
            symbols = new List<MusicalSymbol>();
            currentNote = rootNote;

            while (currentNote != null)
            {
                DoClef();
                DoTimesig();
                if (actions.ContainsKey(currentNote.GetType()))
                {
                    symbols.Add(actions[currentNote.GetType()]());
                }
                DoBarline();
                currentNote = currentNote.nextSymbol;
            }

            return symbols;
        }

        private MusicalSymbol DoNote()
        {
            Dictionary<float, MusicalSymbolDuration> durriation = new Dictionary<float, MusicalSymbolDuration>()
            {
                {1, MusicalSymbolDuration.Whole },
                {2, MusicalSymbolDuration.Half },
                {4, MusicalSymbolDuration.Quarter },
                {8,MusicalSymbolDuration.Eighth },
                {16, MusicalSymbolDuration.Sixteenth },
                {32, MusicalSymbolDuration.d32nd },
                {64, MusicalSymbolDuration.d64th },
                {128, MusicalSymbolDuration.d128th }
            };
            ClassLibrary.Note cr = (ClassLibrary.Note)currentNote;
            PSAMControlLibrary.Note n = new PSAMControlLibrary.Note(cr.Pitch, 0, 4, durriation[cr.Duration],
            NoteStemDirection.Up, NoteTieType.None,
            new List<NoteBeamType>() { NoteBeamType.Single });


            barlinecount += (1 / cr.Duration);
            return n;
        }

        private void DoTimesig()
        {
            ClassLibrary.Note c = (ClassLibrary.Note)currentNote;

            if (TimeSigHasChanged(c))
            {
                symbols.Add(new PSAMControlLibrary.TimeSignature(TimeSignatureType.Numbers, (uint)c.TimeSignature.NumberOfBeats, (uint)c.TimeSignature.TimeOfBeats));
                this.timeSignature.TimeOfBeats = c.TimeSignature.TimeOfBeats;
                this.timeSignature.NumberOfBeats = c.TimeSignature.NumberOfBeats;
            }
        }

        private bool TimeSigHasChanged(ClassLibrary.Note c)
        {
            return this.timeSignature.NumberOfBeats != c.TimeSignature.NumberOfBeats || this.timeSignature.TimeOfBeats != c.TimeSignature.TimeOfBeats;
        }

        private void DoClef()
        {
            ClassLibrary.Note c = (ClassLibrary.Note)currentNote;

            if (ClefHasChanged(c))
            {
                symbols.Add(new PSAMControlLibrary.Clef(clefs[c.Clef.key], 2));
                this.clef.key = c.Clef.key;
            }
        }
        private bool ClefHasChanged(ClassLibrary.Note c)
        {
            return c.Clef.key != this.clef.key;
        }

        private void DoBarline()
        {
            if (!ShouldDoBarline()) return;

            Barline br = new Barline();
            symbols.Add(br);
            barlinecount = 0;
        }

        private bool ShouldDoBarline()
        {
            return barlinecount == 1;
        }

    }
}
