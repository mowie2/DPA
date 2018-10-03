using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using DPA_Musicsheets.Builders;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Managers
{
    public class MidiReader
    {
        List<MidiEvent> openNotes;
        NoteBuilder noteBuilder;

        int division;
        TimeSignature currentTimeSignature;
        Note previousNote;
        int previousMidiKey;

        public MidiReader()
        {
            noteBuilder = new NoteBuilder();
            openNotes = new List<MidiEvent>();
        }

        public void readFile(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);
            processFile(midiSequence);
        }

        public void processFile(Sequence midiSequence)
        {
            division = midiSequence.Division;
            //builder code: set clef G
            foreach (var track in midiSequence)
            {
                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    if (midiMessage.GetType() == typeof(MetaMessage))
                        handleMetaMessage(midiMessage);
                    if (midiMessage.GetType() == typeof(ChannelMessage))
                        handleChannelMessage(midiEvent);
                }
            }
        }

        private void handleChannelMessage(MidiEvent midiEvent)
        {
            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
            if (channelMessage.Command == ChannelCommand.NoteOn)
            {
                if(channelMessage.Data2 > 0)
                {
                    openNotes.Add(midiEvent);
                } else if (channelMessage.Data2 == 0)
                {
                    //Find channelMessage with same height
                    for (int i = 0; i < openNotes.Count; i++)
                    {
                        ChannelMessage previousChannelMessage = openNotes[i].MidiMessage as ChannelMessage;
                        if(previousChannelMessage.Data1 == channelMessage.Data1)
                        {
                            handleNote(openNotes[i], midiEvent);
                        }
                    }
                }
            }
        }

        private void handleNote(MidiEvent previousMidiEvent, MidiEvent midiEvent)
        {
            setNoteDuration(previousMidiEvent.AbsoluteTicks, midiEvent.AbsoluteTicks, division, currentTimeSignature.NumberOfBeats, currentTimeSignature.TimeOfBeats);

            ChannelMessage channelMessage = midiEvent.MidiMessage as ChannelMessage;
            if (previousMidiKey == 0)
                setNotePitch(60, channelMessage.Data1);
            else
                setNotePitch(previousMidiKey, channelMessage.Data1);
            openNotes.Remove(previousMidiEvent);
        }

        private void setNotePitch(int previousMidiKey, int midiKey)
        {
            int octave = (midiKey / 12) - 1;
            switch (midiKey % 12)
            {
                case 0:
                    noteBuilder.SetPitch("c");
                    break;
                case 1:
                    noteBuilder.SetPitch("c");
                    noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                    break;
                case 2:
                    noteBuilder.SetPitch("d");
                    break;
                case 3:
                    noteBuilder.SetPitch("d");
                    noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                    break;
                case 4:
                    noteBuilder.SetPitch("e");
                    break;
                case 5:
                    noteBuilder.SetPitch("f");
                    break;
                case 6:
                    noteBuilder.SetPitch("f");
                    noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                    break;
                case 7:
                    noteBuilder.SetPitch("g");
                    break;
                case 8:
                    noteBuilder.SetPitch("g");
                    noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                    break;
                case 9:
                    noteBuilder.SetPitch("a");
                    break;
                case 10:
                    noteBuilder.SetPitch("a");
                    noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                    break;
                case 11:
                    noteBuilder.SetPitch("b");
                    break;
            }

            int distance = midiKey - previousMidiKey;
            while (distance < -6)
            {
                //verlaag octaaf
                distance += 8;
            }

            while (distance > 6)
            {
                //verhoog octaaf
                distance -= 8;
            }
        }

        private void setNoteDuration(int absoluteTicks, int nextNoteAbsoluteTicks, int division, int beatNote, int beatsPerBar)
        {
            int duration = 0;
            int dots = 0;
            double deltaTicks = nextNoteAbsoluteTicks - absoluteTicks;
            double percentageOfBar = 0;
            

            double percentageOfBeatNote = deltaTicks / division;
            percentageOfBar = (1.0 / beatsPerBar) * percentageOfBeatNote;

            for (int noteLength = 32; noteLength >= 1; noteLength -= 1)
            {
                double absoluteNoteLength = (1.0 / noteLength);

                if (percentageOfBar <= absoluteNoteLength)
                {
                    if (noteLength < 2)
                        noteLength = 2;

                    int subtractDuration;

                    if (noteLength == 32)
                        subtractDuration = 32;
                    else if (noteLength >= 16)
                        subtractDuration = 16;
                    else if (noteLength >= 8)
                        subtractDuration = 8;
                    else if (noteLength >= 4)
                        subtractDuration = 4;
                    else
                        subtractDuration = 2;

                    if (noteLength >= 17)
                        duration = 32;
                    else if (noteLength >= 9)
                        duration = 16;
                    else if (noteLength >= 5)
                        duration = 8;
                    else if (noteLength >= 3)
                        duration = 4;
                    else
                        duration = 2;

                    double currentTime = 0;

                    while (currentTime < (noteLength - subtractDuration))
                    {
                        var addtime = 1 / ((subtractDuration / beatNote) * Math.Pow(2, dots));
                        if (addtime <= 0) break;
                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4) break;
                    }

                    break;
                }
            }

            noteBuilder.SetDuriation(duration);
            noteBuilder.SetDotted(dots);
        }

        private void handleMetaMessage(IMidiMessage midiMessage)
        {
            var metaMessage = midiMessage as MetaMessage;
            if (metaMessage.MetaType == MetaType.TimeSignature)
            {
                handleTimeSignature(metaMessage);
            }
            else if (metaMessage.MetaType == MetaType.Tempo)
            {
                handleTempo(metaMessage);
            }
        }

        private void handleTimeSignature(MetaMessage metaMessage)
        {
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            var _beatNote = timeSignatureBytes[0];
            var _beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));

            currentTimeSignature = new TimeSignature();
            currentTimeSignature.NumberOfBeats = _beatNote;
            currentTimeSignature.TimeOfBeats = _beatsPerBar;

            noteBuilder.SetTimeSignature(currentTimeSignature);
        }

        private void handleTempo(MetaMessage metaMessage)
        {
            byte[] tempoBytes = metaMessage.GetBytes();
            int tempo = (tempoBytes[0] & 0xff) << 16 | (tempoBytes[1] & 0xff) << 8 | (tempoBytes[2] & 0xff);
            var _bpm = 60000000 / tempo;
            //builder set tempo
        }
    }
}
