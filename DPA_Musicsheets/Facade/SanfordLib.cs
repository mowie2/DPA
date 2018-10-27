using ClassLibrary;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight.CommandWpf;
using Sanford.Multimedia.Midi;
using System;

namespace DPA_Musicsheets.Facade
{
    public class SanfordLib: IMidiPlayer
    {
        public OutputDevice _outputDevice { get; set; }
        private DomainToMidi domainToMidi;
        private Sequencer _sequencer;
        public Sequence MidiSequence
        {
            get { return _sequencer.Sequence; }
            set
            {
                //StopCommand.Execute(null);
                _sequencer.Sequence = value;
                //UpdateButtons();
            }
        }

        public void SetMidisequence(Symbol symbol)
        {
            MidiSequence = domainToMidi.GetMidiSequence(symbol);
        }

        public bool CheckSequence()
        {
            return _sequencer.Sequence != null;
        }

        public void SetSequncerPosition(int position)
        {
            _sequencer.Position = position;
        }

        public void SequencerStop()
        {
            _sequencer.Stop();
        }

        public void ContinueSequence()
        {
            _sequencer.Continue();
        }

        private RelayCommand StopCommand;
        private readonly Action UpdateButtons;

        public SanfordLib(/*RelayCommand stop, Action update*/)
        {
            this._sequencer = new Sequencer();
            this._outputDevice = new OutputDevice(0);

            //this.StopCommand = stop;
            //this.UpdateButtons = update;

            this.domainToMidi = new DomainToMidi();
        }

        public void Cleanup()
        {
            _sequencer.Stop();
            _sequencer.Dispose();
            _outputDevice.Dispose();
        }

        public void SquencePlayCompleted(bool running)
        {
            _sequencer.PlayingCompleted += (playingSender, playingEvent) =>
            {
                _sequencer.Stop();
                running = false;
            };
        }

        public void SequencerChannelMessagedPlayed(EventHandler<ChannelMessageEventArgs> e)
        {
            _sequencer.ChannelMessagePlayed += e;
        }

        public void ChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            try
            {
                _outputDevice.Send(e.Message);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is OutputDeviceException)
            {
                // Don't crash when we can't play
                // We have to do it this way because IsDisposed on
                // _outDevice may be false when it is being disposed
                // so this is the only safe way to prevent race conditions

            }
        }

        public void CreateSequence(string filename)
        {
            Sequence temp = new Sequence();
            temp.Load(filename);

            this.MidiSequence = temp;
        }
    }
}
