using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;

namespace DPA_Musicsheets.Managers
{
    public class MidiReader
    {
        Queue<MidiEvent> openNotes;

        public void readFile(string fileName)
        {
            Sequence midiSequence = new Sequence();
            midiSequence.Load(fileName);
            processFile(midiSequence);
        }

        public void processFile(Sequence midiSequence)
        {
            //builder code: set clef G
            foreach (var track in midiSequence)
            {
                foreach (var midiEvent in track.Iterator())
                {
                    IMidiMessage midiMessage = midiEvent.MidiMessage;
                    if (midiMessage.GetType() == typeof(MetaMessage))
                        handleMetaMessage(midiMessage);
                    if (midiMessage.GetType() == typeof(ChannelMessage))
                        handleChannelMessage(midiMessage);
                }
            }
        }

        private void handleChannelMessage(IMidiMessage midiMessage)
        {
            var channelMessage = midiMessage as ChannelMessage;
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

            }
        }

        private void handleTimeSignature(MetaMessage metaMessage)
        {
            byte[] timeSignatureBytes = metaMessage.GetBytes();
            var _beatNote = timeSignatureBytes[0];
            var _beatsPerBar = (int)(1 / Math.Pow(timeSignatureBytes[1], -2));
        }

        private void handleTempo(MetaMessage metaMessage)
        {

        }
    }
}
