using DomainModel;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Facade
{
    public class PsamContolLib
    {
        private List<MusicalSymbol> symbols;

        private Symbol currentNote;
        private readonly DomainModel.TimeSignature timeSignature;
        private readonly DomainModel.Clef clef;
        private float barlinecount = 0;

        private Dictionary<Type, Action> actions;
        private readonly Dictionary<DomainModel.Clef.Key, ClefType> clefs;
        private readonly Dictionary<DomainModel.BarLine.TYPE, RepeatSignType> reapeatType;
        private readonly Dictionary<float, MusicalSymbolDuration> durriation;
        private Dictionary<Semitone.SEMITONE, int> semitones;
        public PsamContolLib()
        {
            durriation = new Dictionary<float, MusicalSymbolDuration>()
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
            actions = new Dictionary<Type, Action>
            {
                { typeof(DomainModel.Note), DoNote },
                //{ typeof(ClassLibrary.BarLine), DoBarline },
                { typeof(DomainModel.BarLine), DoRepeat },
               // { typeof(ClassLibrary.TimeSignature), DoTimesig },
               // {typeof(ClassLibrary.al) }
            };


            clefs = new Dictionary<DomainModel.Clef.Key, ClefType>
            {
                { DomainModel.Clef.Key.C, ClefType.CClef },
                { DomainModel.Clef.Key.F, ClefType.FClef },
                { DomainModel.Clef.Key.G, ClefType.GClef }
            };
            clef = new DomainModel.Clef();
            reapeatType = new Dictionary<BarLine.TYPE, RepeatSignType>
            {
                { DomainModel.BarLine.TYPE.NORMAL, RepeatSignType.None },
                { DomainModel.BarLine.TYPE.START, RepeatSignType.Forward },
                { DomainModel.BarLine.TYPE.REPEAT, RepeatSignType.Backward }
            };
            timeSignature = new DomainModel.TimeSignature();
            semitones = new Dictionary<Semitone.SEMITONE, int>
            {
                { Semitone.SEMITONE.MAJOR, -1 },
                { Semitone.SEMITONE.MINOR, 1 },
                { Semitone.SEMITONE.NORMAL, 0 }
            };
        }

        
        public IList<MusicalSymbol> GetStaffsFromTokens(Symbol rootNote)
        {
            if (rootNote == null) return new List<MusicalSymbol>();
            clef.key = DomainModel.Clef.Key.NOTSET;
            timeSignature.NumberOfBeats = 0;
            timeSignature.TimeOfBeats = 0;
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
            DomainModel.Note cr = (DomainModel.Note)currentNote;

            if(cr.Pitch == "")
            {
                Rest r = new Rest(durriation[cr.Duration]);
                r.NumberOfDots = cr.Dotted;
                symbols.Add(r);
                return;
            }

            PSAMControlLibrary.Note n = new PSAMControlLibrary.Note(cr.Pitch.ToUpper(), semitones[cr.Semitone], 2 + cr.Octave, durriation[cr.Duration],
            NoteStemDirection.Up, NoteTieType.None,
            new List<NoteBeamType>() { NoteBeamType.Single });
            n.NumberOfDots = cr.Dotted;

            float count = 1 / cr.Duration;
            float dur = (float)((Math.Pow(2, cr.Dotted) - 1) / Math.Pow(2, cr.Dotted)) + 1;
            barlinecount += (count * dur);
            symbols.Add(n);
        }
        private void DoTimesig()
        {
            if (currentNote.GetType() != typeof(DomainModel.Note)) return;
            DomainModel.Note c = (DomainModel.Note)currentNote;

            if (!TimeSigHasChanged(c)) return;
            this.timeSignature.TimeOfBeats = c.TimeSignature.TimeOfBeats;
            this.timeSignature.NumberOfBeats = c.TimeSignature.NumberOfBeats;
            symbols.Add(new PSAMControlLibrary.TimeSignature(TimeSignatureType.Numbers, (uint)c.TimeSignature.NumberOfBeats, (uint)c.TimeSignature.TimeOfBeats));
        }

        private bool TimeSigHasChanged(DomainModel.Note c)
        {

            if(c.TimeSignature == null)
            {
                return false;
            }
            return this.timeSignature.NumberOfBeats != c.TimeSignature.NumberOfBeats || this.timeSignature.TimeOfBeats != c.TimeSignature.TimeOfBeats;
        }

        private void DoClef()
        {
            if (currentNote.GetType() != typeof(DomainModel.Note)) return;
            DomainModel.Note c = (DomainModel.Note)currentNote;

            if (!ClefHasChanged(c)) return;

            symbols.Add(new PSAMControlLibrary.Clef(clefs[c.Clef.key], 2));

            // return new PSAMControlLibrary.Clef(clefs[c.Clef.key], 2);
            this.clef.key = c.Clef.key;

        }
        private bool ClefHasChanged(DomainModel.Note c)
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
            if (currentNote.nextSymbol != null && currentNote.nextSymbol.GetType() == typeof(BarLine))
            {
                return false;
            }

            return barlinecount == 1;
        }

        public void DoRepeat()
        {
            DomainModel.BarLine br = (DomainModel.BarLine)currentNote;

            Barline b = new Barline();

            //b.RepeatSign = reapeatType[br.Type];

            if (br.Type == BarLine.TYPE.REPEAT)
            {
                int alt = 1;
                Barline startAlt = new Barline();
                startAlt.AlternateRepeatGroup = alt;
                symbols.Add(startAlt);

                for (int i = 0; i < br.Alternatives.Count; i++)
                {
                    Symbol temp = br.Alternatives[i];
                    while (temp != null)
                    {
                        DomainModel.Note cr = (DomainModel.Note)temp;
                        PSAMControlLibrary.Note n = new PSAMControlLibrary.Note(cr.Pitch.ToUpper(), 0, 2 + cr.Octave, durriation[cr.Duration],
                        NoteStemDirection.Up, NoteTieType.None,
                        new List<NoteBeamType>() { NoteBeamType.Single });
                        symbols.Add(n);
                        temp = temp.nextSymbol;
                    }

                    if (i == 0)
                    {
                        alt++;
                        Barline blt = new Barline();
                        blt.RepeatSign = RepeatSignType.Backward;
                        blt.AlternateRepeatGroup = alt;
                        symbols.Add(blt);

                        continue;
                    }

                    //Barline endAlt = new Barline();
                    //endAlt.RepeatSign = RepeatSignType.None;
                    //endAlt.AlternateRepeatGroup = alt;
                    //symbols.Add(endAlt);
                }
                return;
            }
            b.RepeatSign = reapeatType[br.Type];
            symbols.Add(b);
            barlinecount = 0;

        }
    }
}
