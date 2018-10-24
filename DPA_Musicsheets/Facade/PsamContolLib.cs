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
        private Dictionary<Type, Action> actions;
        private readonly Dictionary<ClassLibrary.Clef.Key, ClefType> clefs;
        private readonly Dictionary<ClassLibrary.BarLine.TYPE, RepeatSignType> reapeatType;
        private readonly ClassLibrary.TimeSignature timeSignature;
        private readonly ClassLibrary.Clef clef;
        private float barlinecount = 0;
        public PsamContolLib()
        {
            actions = new Dictionary<Type, Action>
            {
                { typeof(ClassLibrary.Note), DoNote },
                //{ typeof(ClassLibrary.BarLine), DoBarline },
                { typeof(ClassLibrary.BarLine), DoRepeat },
               // { typeof(ClassLibrary.TimeSignature), DoTimesig },
               // {typeof(ClassLibrary.al) }
            };

            clefs = new Dictionary<ClassLibrary.Clef.Key, ClefType>
            {
                { ClassLibrary.Clef.Key.C, ClefType.CClef },
                { ClassLibrary.Clef.Key.F, ClefType.FClef },
                { ClassLibrary.Clef.Key.G, ClefType.GClef }
            };
            clef = new ClassLibrary.Clef();
            reapeatType = new Dictionary<BarLine.TYPE, RepeatSignType>
            {
                { ClassLibrary.BarLine.TYPE.NORMAL, RepeatSignType.None },
                { ClassLibrary.BarLine.TYPE.START, RepeatSignType.Forward },
                { ClassLibrary.BarLine.TYPE.REPEAT, RepeatSignType.Backward }
            };
            timeSignature = new ClassLibrary.TimeSignature();


        }

        public IList<MusicalSymbol> GetStaffsFromTokens(Symbol rootNote)
        {
            symbols = new List<MusicalSymbol>();
            currentNote = rootNote;
            DoClef();
            DoTimesig();
            while (currentNote != null)
            {
                //DoClef();
                //DoTimesig();
                if (actions.ContainsKey(currentNote.GetType()))
                {
                    actions[currentNote.GetType()]();
                }
                DoBarline();
                currentNote = currentNote.nextSymbol;
            }

            return symbols;
        }

        private void DoNote()
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


            float count = 1 / cr.Duration;
            float dur = (float)((Math.Pow(2, cr.Dotted) - 1) / Math.Pow(2, cr.Dotted)) + 1;
            barlinecount += (count * dur);
            symbols.Add(n);
        }

        private void DoTimesig()
        {
            if (currentNote.GetType() != typeof(ClassLibrary.Note)) return;
            ClassLibrary.Note c = (ClassLibrary.Note)currentNote;

            if (!TimeSigHasChanged(c)) return;
            this.timeSignature.TimeOfBeats = c.TimeSignature.TimeOfBeats;
            this.timeSignature.NumberOfBeats = c.TimeSignature.NumberOfBeats;
            symbols.Add(new PSAMControlLibrary.TimeSignature(TimeSignatureType.Numbers, (uint)c.TimeSignature.NumberOfBeats, (uint)c.TimeSignature.TimeOfBeats));
        }

        private bool TimeSigHasChanged(ClassLibrary.Note c)
        {
            return this.timeSignature.NumberOfBeats != c.TimeSignature.NumberOfBeats || this.timeSignature.TimeOfBeats != c.TimeSignature.TimeOfBeats;
        }

        private void DoClef()
        {
            if (currentNote.GetType() != typeof(ClassLibrary.Note)) return;
            ClassLibrary.Note c = (ClassLibrary.Note)currentNote;

            if (!ClefHasChanged(c)) return;

            symbols.Add(new PSAMControlLibrary.Clef(clefs[c.Clef.key], 2));

            // return new PSAMControlLibrary.Clef(clefs[c.Clef.key], 2);
            this.clef.key = c.Clef.key;

        }
        private bool ClefHasChanged(ClassLibrary.Note c)
        {
            if (c.Clef == null) return false;
            return c.Clef.key != this.clef.key;
        }

        private void DoBarline()
        {
            if (!ShouldDoBarline()) return;
            // ClassLibrary.BarLine n = (ClassLibrary.BarLine)currentNote;
            Barline br = new Barline();
            br.RepeatSign = RepeatSignType.None;
            //br.RepeatSign = reapeatType[n.Type];
            barlinecount = 0;
            symbols.Add(br);
        }

        private bool ShouldDoBarline()
        {
            return barlinecount == 1;
        }

        public void DoRepeat()
        {
            ClassLibrary.BarLine br = (ClassLibrary.BarLine)currentNote;

            Barline b = new Barline();

            b.RepeatSign = reapeatType[br.Type];

            symbols.Add(b);
            barlinecount = 0;

        }
    }
}
