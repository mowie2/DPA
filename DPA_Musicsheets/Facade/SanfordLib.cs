using ClassLibrary;
using ClassLibrary.Interfaces;
using DomainModel;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight.CommandWpf;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DPA_Musicsheets.Facade
{
    public class SanfordLib: IMusicPlayer
    {
        public OutputDevice _outputDevice { get; set; }
        private IConvertToExtention converter;
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

        public void Rewind()
        {
            SetSequncerPosition(0);
        }

        public bool Check()
        {
            return _sequencer.Sequence != null;
        }

        private void SetSequncerPosition(int position)
        {
            _sequencer.Position = position;
        }

        public void Stop()
        {
            _sequencer.Stop();
        }

        public void Continue()
        {
            _sequencer.Continue();
        }

        //private RelayCommand StopCommand;
        //private readonly Action UpdateButtons;

        public SanfordLib(/*RelayCommand stop, Action update*/)
        {
            this._sequencer = new Sequencer();
            this._outputDevice = new OutputDevice(0);

            _sequencer.ChannelMessagePlayed += ChannelMessagePlayed;
            SequencerChannelMessagedPlayed(this.ChannelMessagePlayed);

            //this.StopCommand = stop;
            //this.UpdateButtons = update;

            ConverterGetter converterGetter = new ConverterGetter();
            converter = converterGetter.GetConvertToExtention(".mid");
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

        public void SetMusic(Symbol symbol)
        {
            Stop();
            Rewind();
            if (converter == null) return;
            MidiSequence = converter.Convert(symbol) as Sequence;
        }
    }
}
