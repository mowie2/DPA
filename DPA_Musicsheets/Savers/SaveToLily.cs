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
        private bool setOctave;

        public SaveToLily()
        {
            lilyString = "";
            currentClef = null;
            currentTimeSignature = null;
            currentDuration = 0;
            CurrentBarTime = 0;
            currentTempo = null;
            currentOctave = 0;
            setOctave = false;

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
            while (currentSymbol != null)
            {
                currentSymbol = (Symbol)writeLilyLookupTable[currentSymbol.GetType()].DynamicInvoke(currentSymbol);
            }
            lilyString += "}";
        }

        public string WriteRelative(int octaveModifier)
        {
            if (!setOctave)
            {
                setOctave = true;
                return "\\relative c" + WriteOctaveModifier(octaveModifier) + "{\n";
            }
            return "";
        }

        public string WriteOctaveModifier(int octaveModifier)
        {
            string returnString = "";
            int currentOctaveCopy = currentOctave;
            for (int i = 0; i < Math.Abs(octaveModifier - currentOctaveCopy); i++)
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
                currentTempo = tempo;
                returnString = "\\tempo " + tempo.noteDuration + "=" + tempo.bpm + "\n";
            }
            return returnString;
        }

        public Symbol WriteSection(Symbol startSymbol)
        {
            Symbol currentSymbol = startSymbol;
            while (currentSymbol != null && currentSymbol.GetType() == typeof(Note))
            {
                Note n = currentSymbol as Note;
                lilyString += WriteRelative(n.Octave);
                currentSymbol = WriteNote(n);
                currentSymbol = currentSymbol.nextSymbol;
            }
            return currentSymbol;
        }

        public Symbol WriteRepeat(Symbol startSymbol)
        {
            currentDuration = 0;
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
            lilyString += "\\Alternative {\n";
            if (barline.Alternatives.Count > 0)
            {
                foreach(Note note in barline.Alternatives)
                {
                    currentDuration = 0;
                    lilyString += "{";
                    WriteSection(note);
                    lilyString += "}\n";
                }
            }
            lilyString += "}\n";
        }

        public string WriteBarlines(int duration,int dotted)
        {
            float newDuration = 1 / (float)duration;
            float durationModifier = (float)((Math.Pow(2,dotted)) - 1) / (float)((Math.Pow(2, dotted)))+1;
            newDuration *= durationModifier;

            if (currentDuration+newDuration == CurrentBarTime)
            {
                currentDuration = 0;
                return "|\n";
            } else if(currentDuration + newDuration > CurrentBarTime)
            {
                currentDuration = 0;
            }

            currentDuration += newDuration;
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
            returnString += " ";
            returnString += WriteBarlines((int)note.Duration, (int)note.Dotted);
            lilyString += returnString;
            return note;
        }
    }
}
