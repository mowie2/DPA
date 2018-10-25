using ClassLibrary;
using DPA_Musicsheets.Interfaces;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Savers
{
    public class SaveToMidi : ISavable
    {
        private TimeSignature currentTimeSignature;
        private Tempo currentTempo;
        private int currentTick;
        private int PPQN;
        private BarLine activeBarline;
        private int alternativeCount;
        Sequence sequence;


        public SaveToMidi()
        {
            currentTick = 0;
        }
        public void Save(string fileName, Symbol symbol)
        {
            sequence = new Sequence();
            sequence.Add(new Track());
            sequence.Add(new Track());

            readSymbolTillNull(symbol);
            foreach (var item in sequence)
            {
                item.Insert(currentTick, MetaMessage.EndOfTrackMessage);
            }

            sequence.Save("testfile.mid");
        }

        private void readSymbolTillNull(Symbol symbol)
        {
            Symbol currentSymbol = symbol;
            while (currentSymbol != null)
            {
                if (currentSymbol.GetType() == typeof(Note))
                    addNote(currentSymbol as Note);
                if (currentSymbol.GetType() == typeof(BarLine))
                    handleBarline(currentSymbol as BarLine);
                currentSymbol = currentSymbol.nextSymbol;
            }
        }

        public void handleBarline(BarLine barLine)
        {
            if (activeBarline != barLine)
            {
                activeBarline = barLine;
                alternativeCount = 0;
            } else
                alternativeCount++;
            if (alternativeCount > barLine.Alternatives.Count -1)
                return;

            readSymbolTillNull(barLine.Alternatives[alternativeCount]);
            readSymbolTillNull(barLine.Buddy.nextSymbol);
        }

        public void addNote(Note note)
        {
            addTimeSignature(note.TimeSignature);
            addTempo(note.Tempo);

            if(note.Pitch == "")
            {
                currentTick += calculateNewTick(note.Duration, note.Dotted);
                return;
            }

            
            int midiPitch = calculatePitch(note.Pitch, note.Octave, note.Clef, note.Semitone);
            sequence[1].Insert(currentTick, new ChannelMessage(ChannelCommand.NoteOn, 1, midiPitch, 90));

            setEndOfNote(note.Duration, note.Dotted, midiPitch);
        }

        private void setEndOfNote(float duration, int dotted, int midiPitch)
        {
            //Pulse Length = (BPM * PPQN) / 60 = 120 *192 / 60
            currentTick += calculateNewTick(duration, dotted);

            sequence[1].Insert(currentTick, new ChannelMessage(ChannelCommand.NoteOn, 1, midiPitch, 0));
        }

        private int calculateNewTick(float duration, int dotted)
        {
            float durationModifier = (float)((Math.Pow(2, dotted)) - 1) / (float)((Math.Pow(2, dotted))) + 1;
            ////////////////////////////////////////////////////////

            double absoluteLength = 1.0 / (double)duration;
            absoluteLength += (absoluteLength / 2.0) * dotted;

            double relationToQuartNote = 4 / 4.0;
            double percentageOfBeatNote = (1.0 / 4) / absoluteLength;
            double deltaTicks = (sequence.Division / relationToQuartNote) / percentageOfBeatNote;
            
            return (int) deltaTicks;
        }

        private void addTempo(Tempo tempo)
        {
            if (currentTempo == tempo || tempo == null)
            {
                if (currentTempo == null)
                    addTempo(new Tempo() { bpm = 120 });
                return;
            }
            else
                currentTempo = tempo;
            
            int speed = (60000000 / currentTempo.bpm);
            byte[] newTempo = new byte[3];
            newTempo[0] = (byte)((speed >> 16) & 0xff);
            newTempo[1] = (byte)((speed >> 8) & 0xff);
            newTempo[2] = (byte)(speed & 0xff);

            sequence[0].Insert(currentTick, new MetaMessage(MetaType.Tempo, newTempo));
        }

        private void addTimeSignature(TimeSignature timeSignature)
        {
            if (currentTimeSignature == timeSignature || timeSignature == null)
            {
                if (currentTimeSignature == null)
                    addTimeSignature(new TimeSignature() { NumberOfBeats = 4, TimeOfBeats = 4 });
                return;
            }
            else
                currentTimeSignature = timeSignature;

            TimeSignatureBuilder timeSignatureBuilder = new TimeSignatureBuilder();
            timeSignatureBuilder.Numerator = (byte)currentTimeSignature.TimeOfBeats;
            timeSignatureBuilder.Denominator = (byte)currentTimeSignature.NumberOfBeats;
            timeSignatureBuilder.ClocksPerMetronomeClick = 24;
            timeSignatureBuilder.ThirtySecondNotesPerQuarterNote = 8;
            timeSignatureBuilder.Build();

            sequence[0].Insert(currentTick, timeSignatureBuilder.Result);
        }

        private int calculatePitch(string pitch, int octave, Clef clef, Semitone.SEMITONE semitone)
        {
            Dictionary<string, int> pitchDictionary = new Dictionary<string, int>()
            {
                {"c", 0 },
                {"d", 2 },
                {"e", 4 },
                {"f", 5 },
                {"g", 7 },
                {"a", 9 },
                {"b", 11 }
            };
            int midiKey;
            pitchDictionary.TryGetValue(pitch, out midiKey);
            if (semitone == Semitone.SEMITONE.MINOR)
                midiKey++;
            if (semitone == Semitone.SEMITONE.MAJOR)
                midiKey--;
            midiKey += octave * 12;

            if (midiKey > 127)
                return 0;
            if (midiKey < 0)
                return 0;

            return midiKey+60;
        }
    }
}
