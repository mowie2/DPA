﻿using ClassLibrary;
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
        private ChannelMessageBuilder builder;
        private List<Track> tracks;
        private BarLine activeBarline;
        private int alternativeCount;


        public SaveToMidi()
        {
            builder = new ChannelMessageBuilder();
            tracks.Add(new Track());
            tracks.Add(new Track());
            currentTick = 0;
            PPQN = 192;
        }
        public void Save(string fileName, Symbol symbol)
        {
            readSymbolTillNull(symbol);
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
            if (alternativeCount > barLine.Alternatives.Count)
                return;

            readSymbolTillNull(barLine.Alternatives[alternativeCount]);
        }

        public void addNote(Note note)
        {
            addTimeSignature(note.TimeSignature);
            addTempo(note.Tempo);

            builder.MidiChannel = 0;
            int midiPitch = calculatePitch(note.Pitch, note.Octave, note.Clef, note.Semitone);
            builder.Data1 = midiPitch;
            builder.Data2 = 90;
            builder.Build();
            tracks[1].Insert(currentTick, builder.Result);

            setEndOfNote(note.Duration, note.Dotted, midiPitch);
        }

        private void setEndOfNote(float duration, int dotted, int midiPitch)
        {
            //Pulse Length = (BPM * PPQN) / 60 = 120 *192 / 60
            builder.MidiChannel = 0;
            builder.Data1 = midiPitch;
            builder.Data2 = 0;
            builder.Build();
            
            double endSequence = 1 / duration * 4;
            endSequence = endSequence * ((Math.Pow(2, dotted) - 1) / (Math.Pow(2, dotted) + endSequence));

            int pulseLength =(int) (PPQN * endSequence) * 120 / 60;
            currentTick += pulseLength;

            tracks[1].Insert(currentTick, builder.Result);
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
