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
        private readonly Dictionary<Type,Delegate> writeLilyLookupTable;
        private readonly Dictionary<Semitone.SEMITONE,string> pitchModifiers;


    private Clef currentClef;
        public string lilyString;
        private TimeSignature currentTimeSignature;
        private Tempo currentTempo;
        private int currentOctave;

        public SaveToLily()
        {
            lilyString = "";
            currentTimeSignature = null;
            currentTempo = null;
            currentOctave = 0;

            writeLilyLookupTable = new Dictionary<Type, Delegate>
            {
                [typeof(Note)] = new Func<Symbol,Symbol>(WriteBarlines),
                [typeof(BarLine)] = new Func<Symbol, Symbol>(WriteRepeat)
            };

            pitchModifiers= new Dictionary<Semitone.SEMITONE, string>
            {
                [Semitone.SEMITONE.MAJOR] = "es",
                [Semitone.SEMITONE.MINOR] = "is",
                [Semitone.SEMITONE.NORMAL] = "" 
            };
        }

        public void Save(string fileName, Symbol rootSymbol)
        {
            Symbol currentSymbol = rootSymbol;
            while (true)
            {
                currentSymbol = (Symbol)writeLilyLookupTable[currentSymbol.GetType()].DynamicInvoke(currentSymbol);
                if (currentSymbol.nextSymbol == null)
                {
                    break;
                }
            }
        }

        public string WriteOctaveModifier(int octaveModifier)
        {
            string returnString = "";
            bool bigger = currentOctave > octaveModifier;
            for (int i = 0; i < Math.Abs(octaveModifier - currentOctave); i++)
            {
                if (bigger)
                {
                    returnString += "\'";
                }
                else
                {
                    returnString += ",";
                }
            }
            return returnString;
        }

        public string WriteClef(Clef clef)
        {
            string returnString = "";
            if (clef != currentClef)
            {
                returnString = "\\clef " + clef.key.ToString() + "\n";
            }
            return returnString;
        }

        public string WriteTimeSignature(TimeSignature timeSignature)
        {
            string returnString = "";
            if (timeSignature != currentTimeSignature)
            {
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



        public Symbol WriteRepeat(Symbol startSymbol)
        {
            Symbol currentSymbol = startSymbol.nextSymbol; 
            lilyString += "\\repeat volta 2 {\n";
            while (currentSymbol.GetType() != typeof(BarLine) && currentSymbol.GetType() == typeof(Note))
            {
                currentSymbol = WriteBarlines(currentSymbol);
            }
            lilyString += "}\n";
            return currentSymbol;
        }

        public Symbol WriteBarlines(Symbol StartSymbol)
        {
            Symbol currentSymbol = StartSymbol;
            int endTime = currentTimeSignature.NumberOfBeats * currentTimeSignature.TimeOfBeats;
            while (endTime>0 && (currentSymbol.nextSymbol.GetType() == typeof(Note)))
            {
                Note n = (Note)currentSymbol;
                endTime -= (int)n.Duration;
                WriteNote(n);
                currentSymbol = currentSymbol.nextSymbol;
            }
            lilyString += "|\n";
            return currentSymbol;
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


        public void WriteNote(Note note)
        {
            string returnString = "";
            returnString += WriteClef(note.Clef);
            returnString += WriteTimeSignature(note.TimeSignature);
            returnString += WriteTempo(note.Tempo);
            returnString += WritePitch(note.Pitch);
            returnString += pitchModifiers[note.Semitone];
            returnString += WriteOctaveModifier(note.Octave);
            returnString += WriteDotted(note.Dotted);
            returnString += note.Duration;
            lilyString += returnString+" ";
        }
    }
}
