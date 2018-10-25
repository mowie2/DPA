using ClassLibrary;
using DPA_Musicsheets.Builders;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Savers;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Managers
{
    public class MidiReader : IReader
    {
        private Dictionary<int, Tuple<MidiEvent,Note>> openNotes;
        private readonly NoteBuilder noteBuilder;
        private int division;
        private TimeSignature currentTimeSignature;
        private Note firstNote;
        private Note prevNote;
        private int lastAbsoluteTicks;

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

        public Symbol readFile(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);
            processFile(midiSequence);
            SaveToLily s = new SaveToLily();
            s.Save("test.ly",firstNote);
            return firstNote;
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
            sortAllEvents(allEventsArray);

            noteBuilder.SetClef(new Clef(Clef.Key.G));
            lastAbsoluteTicks = 0;
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
            int right = middle+1;
            MidiEvent[] secList = new MidiEvent[(high - low) + 1];
            int tempIndex = 0;

            while (left <= middle && right <= high)
            {
                if (list[left].AbsoluteTicks < list[right].AbsoluteTicks)
                {
                    secList[tempIndex] = list[left];
                    left++;
                }
                else if (list[left].AbsoluteTicks > list[right].AbsoluteTicks)
                {
                    secList[tempIndex] = list[right];
                    right++;
                }
                else
                {
                    if (list[left].GetType() == list[right].GetType())
                    {
                        if (list[left].DeltaTicks < list[right].DeltaTicks)
                        {
                            secList[tempIndex] = list[right];
                            right++;
                        }
                        else
                        {
                            secList[tempIndex] = list[left];
                            left++;
                        }
                    }
                    else if (list[left].GetType() == typeof(MetaMessage))
                    {
                        secList[tempIndex] = list[left];
                        left++;
                    }
                    else
                    {
                        secList[tempIndex] = list[right];
                        right++;
                    }
                }
                tempIndex++;
            }

            if (left <= middle)
            {
                while (left <= middle)
                {
                    secList[tempIndex] = list[left];
                    left++;
                    tempIndex++;
                }
            }

            if (right <= high)
            {
                while (right <= high)
                {
                    secList[tempIndex] = list[right];
                    right++;
                    tempIndex++;
                }
            }

            double prev = 0;
            for (int i = 0; i < secList.Length; i++)
            {
                if (prev > secList[i].AbsoluteTicks)
                {

                }
                prev = secList[i].AbsoluteTicks;
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
                    createNewNote(midiEvent);
                else if (channelMessage.Data2 == 0)
                    finishOldNote(midiEvent);
            }
        }

        private void createNewNote(MidiEvent midiEvent)
        {
            var channelMessage = midiEvent.MidiMessage as ChannelMessage;

            setNotePitch(channelMessage.Data1);
            Note note = noteBuilder.BuildNote();

            checkRest(midiEvent);

            if (!openNotes.ContainsKey(channelMessage.Data1))
                openNotes.Add(channelMessage.Data1, new Tuple<MidiEvent, Note>(midiEvent, note));
            else
                return;

            connectToLastNote(note);
        }

        private void checkRest(MidiEvent midiEvent)
        {
            if (openNotes.Count == 0)
            {
                if (lastAbsoluteTicks < midiEvent.AbsoluteTicks)
                {
                    addRest(midiEvent.AbsoluteTicks);
                }
            }
        }

        private void addRest(double absoluteTicks)
        {
            Note note = noteBuilder.BuildNote();
            setNoteDuration((absoluteTicks - lastAbsoluteTicks), division, currentTimeSignature.NumberOfBeats, currentTimeSignature.TimeOfBeats, note);

            connectToLastNote(note);
        }

        private void connectToLastNote(Note note)
        {
            if (firstNote != null)
            {
                prevNote.nextSymbol = note;
                prevNote = note;
            }
            else
            {
                firstNote = note;
                prevNote = note;
            }
        }

        private void finishOldNote(MidiEvent midiEvent)
        {
            var channelMessage = midiEvent.MidiMessage as ChannelMessage;

            var tuple = new Tuple<MidiEvent, Note>(null, null);
            if (!openNotes.TryGetValue(channelMessage.Data1, out tuple))
            {
                return;
            };
            setNoteDuration((midiEvent.AbsoluteTicks - tuple.Item1.AbsoluteTicks), division, currentTimeSignature.NumberOfBeats, currentTimeSignature.TimeOfBeats, tuple.Item2);
            lastAbsoluteTicks = midiEvent.AbsoluteTicks;
            openNotes.Remove(channelMessage.Data1);
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
            noteBuilder.ClearOctave();
            int distance = midiKey - previousMidikey;
            while (distance < -6)
            {
                noteBuilder.ModifyOctave(-1);
                distance += 8;
            }

            while (distance > 6)
            {
                noteBuilder.ModifyOctave(1);
                distance -= 8;
            }
        }


        // absoluut ticks: start tijd
        // nextNoteAbosulutetick: eind tijd
        // division: 
        private void setNoteDuration(double deltaTicks, int division, int beatNote, int beatsPerBar, Note note)
        {
            int duration = 0;
            int dots = 0;
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

            note.Duration = duration;
            note.Dotted = dots;
            //noteBuilder.SetDuriation(duration);
            //noteBuilder.SetDotted(dots);
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
