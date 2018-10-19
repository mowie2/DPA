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
        private ChannelMessageBuilder builder;
        private List<Track> tracks;


        public SaveToMidi()
        {
            builder = new ChannelMessageBuilder();
            tracks.Add(new Track());
            tracks.Add(new Track());
            currentTick = 0;
        }
        public void Save(string fileName, Symbol symbol)
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
            throw new NotImplementedException();
        }

        public void addNote(Note note)
        {
            builder.MidiChannel = 0;
            builder.Data1 = calculatePitch(note.Pitch, note.Octave, note.Clef, note.Semitone);
            builder.Data2 = 90;
            builder.Build();
            tracks[1].Insert(currentTick, builder.Result);

            addTimeSignature(note.TimeSignature);
            addTempo(note.Tempo);
            setEndOfNote(note.Duration, note.Dotted);
        }

        private void setEndOfNote(float duration, int dotted)
        {
            
        }

        private void addTempo(Tempo tempo)
        {
            if (currentTempo == tempo)
                return;

            TempoChangeBuilder tempoChangeBuilder = new TempoChangeBuilder();
            tempoChangeBuilder.Tempo = 60000000 / tempo.bpm;
            tempoChangeBuilder.Build();

            tracks[0].Insert(currentTick, builder.Result);
        }

        private void addTimeSignature(TimeSignature timeSignature)
        {
            if (currentTimeSignature != timeSignature)
                return;

            TimeSignatureBuilder timeSignatureBuilder = new TimeSignatureBuilder();
            timeSignatureBuilder.Numerator = (byte)timeSignature.TimeOfBeats;
            timeSignatureBuilder.Denominator = (byte)timeSignature.NumberOfBeats;
            timeSignatureBuilder.ClocksPerMetronomeClick = 24;
            timeSignatureBuilder.ThirtySecondNotesPerQuarterNote = 8;
            builder.Build();

            tracks[0].Insert(currentTick, builder.Result);
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

            return midiKey+60;
        }
    }
}
