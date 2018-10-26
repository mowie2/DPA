﻿using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Converters
{
    class DomainToLily
    {
        private Dictionary<Type, Delegate> writeLilyLookupTable;
        private Dictionary<Semitone.SEMITONE, string> pitchModifiers;
        List<string> notesOrder = new List<string>() { "c", "d", "e", "f", "g", "a", "b" };
        private int prefRelativeOctaveModifier;
        private string prefPitch;
        private Clef currentClef;
        private string lilyString;
        private TimeSignature currentTimeSignature;
        private float currentDuration;
        private float CurrentBarTime;
        private Tempo currentTempo;
        private int currentOctave;
        private bool setOctave;

        public DomainToLily()
        {
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

        public string GetLilyText(Symbol root)
        {
            Clear();
            if (root != null)
            {
                Symbol currentSymbol = root;
                while (currentSymbol != null)
                {
                    currentSymbol = (Symbol)writeLilyLookupTable[currentSymbol.GetType()].DynamicInvoke(currentSymbol);
                }
                lilyString += "}";
            }
            return lilyString;
        }

        private void Clear()
        {
            prefRelativeOctaveModifier = 0;
            prefPitch = "";
            lilyString = "";
            currentClef = null;
            currentTimeSignature = null;
            currentDuration = 0;
            CurrentBarTime = 0;
            currentTempo = null;
            currentOctave = 0;
            setOctave = false;
        }

        private string WriteRelative(int octaveModifier)
        {
            if (!setOctave)
            {
                setOctave = true;
                return "\\relative c" + WriteOctaveModifier(octaveModifier) + "{\r\r\n";
            }
            return "";
        }

        private int RelativeOctaveModifier(string pitch)
        {
            int returnOctave = 0 + prefRelativeOctaveModifier;
            if (!prefPitch.Equals(""))
            {
                int distance = notesOrder.IndexOf(pitch) - notesOrder.IndexOf(prefPitch);
                if (distance > 3)
                {
                    returnOctave += 1;
                }
                else if (distance < -3)
                {
                    returnOctave -= 1;
                }
            }
            prefPitch = pitch;
            prefRelativeOctaveModifier = returnOctave;
            return returnOctave;
        }

        private string WriteOctaveModifier(int octaveModifier)
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
                else if (currentOctave > octaveModifier)
                {
                    returnString += ",";
                    currentOctave--;
                }
            }
            return returnString;
        }

        private string WriteClef(Clef clef)
        {
            string returnString = "";
            if (clef != currentClef)
            {
                currentClef = clef;
                returnString = "\\clef " + clef.key.ToString() + "\r\n";
            }
            return returnString;
        }

        private string WriteTimeSignature(TimeSignature timeSignature)
        {
            string returnString = "";
            if (timeSignature != currentTimeSignature)
            {
                currentTimeSignature = timeSignature;
                currentDuration = 0;
                CurrentBarTime = (float)timeSignature.NumberOfBeats / timeSignature.TimeOfBeats;
                returnString = "\\time " + timeSignature.NumberOfBeats + "/" + timeSignature.TimeOfBeats + "\r\n";
            }
            return returnString;
        }

        private string WriteTempo(Tempo tempo)
        {
            string returnString = "";
            if (tempo != currentTempo)
            {
                currentTempo = tempo;
                returnString = "\\tempo " + tempo.noteDuration + "=" + tempo.bpm + "\r\n";
            }
            return returnString;
        }

        private Symbol WriteSection(Symbol startSymbol)
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

        private Symbol WriteRepeat(Symbol startSymbol)
        {
            currentDuration = 0;
            Symbol currentSymbol = startSymbol.nextSymbol;
            lilyString += "\\repeat volta 2 {\r\n";
            currentSymbol = WriteSection(currentSymbol);
            lilyString += "}\r\n";
            WriteAlternative((BarLine)currentSymbol);
            currentSymbol = currentSymbol.nextSymbol;
            return currentSymbol;
        }

        private void WriteAlternative(BarLine barline)
        {
            lilyString += "\\Alternative {\r\n";
            if (barline.Alternatives.Count > 0)
            {
                foreach (Note note in barline.Alternatives)
                {
                    currentDuration = 0;
                    lilyString += "{";
                    WriteSection(note);
                    lilyString += "}\r\n";
                }
            }
            lilyString += "}\r\n";
        }

        private string WriteBarlines(Note note, int duration, int dotted)
        {
            float newDuration = 1 / (float)duration;
            float durationModifier = (float)((Math.Pow(2, dotted)) - 1) / (float)((Math.Pow(2, dotted))) + 1;
            newDuration *= durationModifier;
            if ((note.nextSymbol != null && note.nextSymbol.GetType() == typeof(BarLine)))
            {
                currentDuration = 0;
                return "\r\n";
            }
            else if (currentDuration + newDuration == CurrentBarTime && note.nextSymbol != null)
            {
                currentDuration = 0;
                return "|\r\n";

            }
            else if (currentDuration + newDuration < CurrentBarTime)
            {
                currentDuration += newDuration;
                return "";
            }
            currentDuration = 0;
            return "";
        }

        private string WritePitch(string pitch)
        {
            if (pitch.Equals(""))
            {
                return "r";
            }
            return pitch;
        }

        private string WriteDotted(int dots)
        {
            string returnString = "";
            for (int i = 0; i < dots; i++)
            {
                returnString += ".";
            }
            return returnString;
        }

        private string WriteDuration(int duration)
        {
            return duration.ToString();
        }

        private Symbol WriteNote(Symbol symbol)
        {
            Note note = (Note)symbol;
            string returnString = "";
            returnString += WriteClef(note.Clef);
            returnString += WriteTimeSignature(note.TimeSignature);
            returnString += WriteTempo(note.Tempo);
            returnString += WritePitch(note.Pitch);
            returnString += pitchModifiers[note.Semitone];
            returnString += WriteOctaveModifier(RelativeOctaveModifier(note.Pitch) + note.Octave);
            returnString += WriteDuration((int)note.Duration);
            returnString += WriteDotted(note.Dotted);
            returnString += " ";
            returnString += WriteBarlines(note, (int)note.Duration, (int)note.Dotted);
            lilyString += returnString;
            return note;
        }
    }
}
