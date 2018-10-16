using ClassLibrary;
using DPA_Musicsheets.Builders;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Managers
{
    public class MidiReader
    {
        private Dictionary<int, Tuple<MidiEvent,Note>> openNotes;
        private readonly NoteBuilder noteBuilder;
        private int division;
        private TimeSignature currentTimeSignature;

        private readonly Dictionary<int, string> pitches;
        List<int> SemitonValues;


        public MidiReader()
        {
            noteBuilder = new NoteBuilder();
            openNotes = new Dictionary<int, Tuple<MidiEvent, Note>>();
            pitches = new Dictionary<int, string>()
            {
                { 0, "c" },
                { 1, "c" },
                { 2, "d" },
                { 3, "d" },
                { 4, "e" },
                { 5, "f" },
                { 6, "f" },
                { 7, "g" },
                { 8, "g" },
                { 9, "a" },
                { 10, "a" },
                { 11, "b" }
            };
            SemitonValues = new List<int>()
            {
                1,
                3,
                6,
                8,
                10,
            };
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
            List<MidiEvent> allEvents = new List<MidiEvent>(); ;

            //add all events to list
            foreach (var track in midiSequence)
            {
                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    if (midiMessage.GetType() == typeof(MetaMessage))
                        addMetaMessage(midiEvent, allEvents);
                    if (midiMessage.GetType() == typeof(ChannelMessage))
                        addChannelMessage(midiEvent, allEvents);
                }
            }

            //sort allNodes
            MidiEvent[] allEventsArray = allEvents.ToArray();
            //sortAllEvents(allEventsArray);

            noteBuilder.SetClef(new Clef(Clef.Key.G));
            foreach (var midiEvent in allEventsArray)
            {
                IMidiMessage midiMessage = midiEvent.MidiMessage;
                if (midiMessage.GetType() == typeof(MetaMessage))
                    handleMetaMessage(midiMessage);
                if (midiMessage.GetType() == typeof(ChannelMessage))
                    handleChannelMessage(midiEvent);
            }
        }

        private void sortAllEvents(MidiEvent[] allEvents)
        {
            mergeSortMidiEvents(allEvents, 0, allEvents.Length - 1);
        }

        private void mergeSortMidiEvents(MidiEvent[] list, int leftIndex, int rightIndex)
        {
            if (leftIndex < rightIndex)
            {
                int middle = (leftIndex / 2) + (rightIndex / 2);
                
                mergeSortMidiEvents(list, leftIndex, middle);
                mergeSortMidiEvents(list, middle + 1, rightIndex);
                Merge(list, leftIndex, middle, rightIndex);
            }
        }

        private void Merge(MidiEvent[] list, int low, int middle, int high)
        {
            int left = low;
            int right = middle;
            MidiEvent[] secList = new MidiEvent[(high - low) + 1];
            int tempIndex = 0;

            System.Diagnostics.Debug.WriteLine("left: " + left + "middle: " + middle + "high: " + high + "sec list length: " + secList.Length);

            if (middle == 38623 && high == 38624)
            {
                return;
            }

            while (left <= middle && right <= high)
            {
                if (list[left].AbsoluteTicks < list[right].AbsoluteTicks)
                {
                    secList[tempIndex] = list[left];
                    left++;
                }
                else
                {
                    //todo:write so metamessages come before channelMessages
                    secList[tempIndex] = list[right];
                    right++;
                }
                tempIndex++;
            }

            if (left >= middle)
            {
                while (right <= high)
                {
                    secList[tempIndex] = list[right];
                    right++;
                    tempIndex++;
                }
            }

            if (right <= high)
            {
                while (left <= middle)
                {
                    secList[tempIndex] = list[left];
                    left++;
                    tempIndex++;
                }
            }
            
            for (int i = 0; i < secList.Length; i++)
            {
                list[low + i] = secList[i];
            }

        }

        private void addChannelMessage(MidiEvent midiEvent, List<MidiEvent> allEvents)
        {
            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
            if (channelMessage.Command == ChannelCommand.NoteOn)
            {
                allEvents.Add(midiEvent);
            }
        }

        private void addMetaMessage(MidiEvent midiEvent, List<MidiEvent> allEvents)
        {
            var metaMessage = midiEvent.MidiMessage as MetaMessage;
            if (metaMessage.MetaType == MetaType.TimeSignature)
            {
                allEvents.Add(midiEvent);
            }
            else if (metaMessage.MetaType == MetaType.Tempo)
            {
                allEvents.Add(midiEvent);
            }
        }

        private void handleChannelMessage(MidiEvent midiEvent)
        {
            var channelMessage = midiEvent.MidiMessage as ChannelMessage;
            if (channelMessage.Command == ChannelCommand.NoteOn)
            {
                if (channelMessage.Data2 > 0)
                {
                    
                    openNotes.Add(channelMessage.Data2, new Tuple<MidiEvent, Note>(midiEvent, new Note()));
                }
                else if (channelMessage.Data2 == 0)
                {
                    //Find channelMessage with same height
                    for (int i = 0; i < openNotes.Count; i++)
                    {
                        ChannelMessage previousChannelMessage = openNotes[i].MidiMessage as ChannelMessage;
                        if (previousChannelMessage.Data1 == channelMessage.Data1)
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
                setNotePitch(channelMessage.Data1);
            else
                setNotePitch(channelMessage.Data1);
            openNotes.Remove(previousMidiEvent);
        }

       
        private void NoteBuilderSetSemitone(int x)
        {
            if (SemitonValues.Contains(x))
            {
                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
            }
        }

        private void setNotePitch(int midiKey)
        {
            int previousMidikey = 60;
            int octave = (midiKey / 12) - 1;

            var x = pitches[midiKey % 12];
            var y = midiKey % 12;
            noteBuilder.SetPitch(pitches[y]);
            NoteBuilderSetSemitone(y);

            #region old code
            /*
                        switch (y)
                        {
                            case 0:

                                break;
                            case 1:
                                noteBuilder.SetPitch(pitches[y]);
                                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                                break;
                            case 2:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                            case 3:
                                noteBuilder.SetPitch(pitches[y]);
                                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                                break;
                            case 4:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                            case 5:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                            case 6:
                                noteBuilder.SetPitch(pitches[y]);
                                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                                break;
                            case 7:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                            case 8:
                                noteBuilder.SetPitch(pitches[y]);
                                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                                break;
                            case 9:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                            case 10:
                                noteBuilder.SetPitch(pitches[y]);
                                noteBuilder.SetSemitone(Semitone.SEMITONE.MINOR);
                                break;
                            case 11:
                                noteBuilder.SetPitch(pitches[y]);
                                break;
                        }
            */
            #endregion
            int distance = midiKey - previousMidikey;
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


        // absoluut ticks: start tijd
        // nextNoteAbosulutetick: eind tijd
        // division: 
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
                        if (addtime <= 0)
                            break;

                        currentTime += addtime;
                        if (currentTime <= (noteLength - subtractDuration))
                        {
                            dots++;
                        }
                        if (dots >= 4)
                            break;
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
