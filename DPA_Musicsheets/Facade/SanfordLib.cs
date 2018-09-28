using GalaSoft.MvvmLight.CommandWpf;
using Sanford.Multimedia.Midi;
using System;

namespace DPA_Musicsheets.Facade
{
    public class SanfordLib
    {
        public OutputDevice _outputDevice { get; set; }
        public Sequencer _sequencer;
        public Sequence MidiSequence
        {
            get { return _sequencer.Sequence; }
            set
            {
                StopCommand.Execute(null);
                _sequencer.Sequence = value;
                UpdateButtons();
            }
        }

        private RelayCommand StopCommand;
        private Action UpdateButtons;
        public SanfordLib(RelayCommand stop, Action update)
        {
            this._sequencer = new Sequencer();
            this._outputDevice = new OutputDevice(0);

            this.StopCommand = stop;
            this.UpdateButtons = update;
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

        public void SetSequence(Sequence sequence)
        {
            this._sequencer.Sequence = sequence;
        }
    }
}
