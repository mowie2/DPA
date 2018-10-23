using ClassLibrary;
using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Savers
{
    public class SaveToLily : ISavable
    {
        private readonly Dictionary<Type, Delegate> writeLilyLookupTable;
        private readonly Dictionary<Semitone.SEMITONE, string> pitchModifiers;

        private Clef currentClef;
        public string lilyString;
        private TimeSignature currentTimeSignature;
        private float currentDuration;
        private float CurrentBarTime;
        private Tempo currentTempo;
        private int currentOctave;

        public SaveToLily()
        {
            lilyString = "";
            currentClef = null;
            currentTimeSignature = null;
            currentDuration = 0;
            CurrentBarTime = 0;
            currentTempo = null;
            currentOctave = 0;

            writeLilyLookupTable = new Dictionary<Type, Delegate>
            {
                [typeof(Note)] = new Func<Symbol, Symbol>(WriteSection),
                [typeof(BarLine)] = new Func<Symbol, Symbol>(WriteRepeat)
            };

            pitchModifiers = new Dictionary<Semitone.SEMITONE, string>
            {
                [Semitone.SEMITONE.MAJOR] = "es",
                [Semitone.SEMITONE.MINOR] = "is",
                [Semitone.SEMITONE.NORMAL] = ""
            };
        }

        public void Save(string fileName, Symbol rootSymbol)
        {
            Symbol currentSymbol = rootSymbol;
            
            currentSymbol = currentSymbol.nextSymbol;
            while (currentSymbol != null)
            {
                currentSymbol = (Symbol)writeLilyLookupTable[currentSymbol.GetType()].DynamicInvoke(currentSymbol);
            }
        }

        public string WriteOctaveModifier(int octaveModifier)
        {
            string returnString = "";
            for (int i = 0; i < Math.Abs(octaveModifier - currentOctave); i++)
            {
                if (currentOctave < octaveModifier)
                {
                    returnString += "\'";
                    currentOctave++;
                }
                else if(currentOctave > octaveModifier)
                {
                    returnString += ",";
                    currentOctave--;
                }
            }
            return returnString;
        }

        public string WriteClef(Clef clef)
        {
            string returnString = "";
            if (clef != currentClef)
            {
                currentClef = clef;
                returnString = "\\clef " + clef.key.ToString() + "\n";
            }
            return returnString;
        }

        public string WriteTimeSignature(TimeSignature timeSignature)
        {
            string returnString = "";
            if (timeSignature != currentTimeSignature)
            {
                currentTimeSignature = timeSignature;
                currentDuration = 0;
                CurrentBarTime = (float)timeSignature.NumberOfBeats/ timeSignature.TimeOfBeats;
                returnString = "\\time " + timeSignature.NumberOfBeats + "/" + timeSignature.TimeOfBeats + "\n";
            }
            return returnString;
        }

        public string WriteTempo(Tempo tempo)
        {
            string returnString = "";
            if (tempo != currentTempo)
            {
                returnString = "\\tempo " + tempo.noteDuration + "=" + tempo.bpm + "\n";
            }
            return returnString;
        }

        public Symbol WriteSection(Symbol startSymbol)
        {
            Symbol currentSymbol = startSymbol;
            while (currentSymbol != null && currentSymbol.GetType() == typeof(Note))
            {
                currentSymbol = WriteNote((Note)currentSymbol);
                currentSymbol = currentSymbol.nextSymbol;
            }
            return currentSymbol;
        }



        public Symbol WriteRepeat(Symbol startSymbol)
        {
            Symbol currentSymbol = startSymbol.nextSymbol;
            lilyString += "\\repeat volta 2 {\n";
            currentSymbol = WriteSection(currentSymbol);
            lilyString += "}\n";
            WriteAlternative((BarLine)currentSymbol);
            currentSymbol = currentSymbol.nextSymbol;
            return currentSymbol;
        }

        public void WriteAlternative(BarLine barline)
        {
            lilyString += "\\Alternative {";
            if (barline.Alternatives.Count > 0)
            {
                foreach(Note note in barline.Alternatives)
                {
                    lilyString += "{";
                    WriteSection(note);
                    lilyString += "}\n";
                }
            }
            lilyString += "}\n";
        }

        public string WriteBarlines()
        {
            if (currentDuration >= CurrentBarTime)
            {
                currentDuration = 0;
                return "|";
            }
            return "";
        }

        public string WritePitch(string pitch)
        {
            if (pitch.Equals(""))
            {
                return "r";
            }
            return pitch;
        }

        public string WriteDotted(int dots)
        {
            string returnString = "";
            for (int i = 0; i < dots; i++)
            {
                returnString += ".";
            }
            return returnString;
        }

        public string WriteDuration(int duration)
        {
            currentDuration += 1/(float)duration;
            return duration.ToString();
        }

        public Note WriteNote(Symbol symbol)
        {
            Note note = (Note)symbol;
            string returnString = "";
            returnString += WriteClef(note.Clef);
            returnString += WriteTimeSignature(note.TimeSignature);
            returnString += WriteTempo(note.Tempo);
            returnString += WritePitch(note.Pitch);
            returnString += pitchModifiers[note.Semitone];
            returnString += WriteOctaveModifier(note.Octave);
            returnString += WriteDotted(note.Dotted);
            returnString += WriteDuration((int)note.Duration);
            returnString += WriteBarlines();
            lilyString += returnString+" ";
            return note;
        }
    }
}
