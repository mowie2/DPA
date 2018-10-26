﻿using DPA_Musicsheets.Facade;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DPA_Musicsheets.ViewModels
{
    /// <summary>
    /// The viewmodel for playing midi sequences.
    /// It supports starting, stopping and restarting.
    /// </summary>
    public class MidiPlayerViewModel : ViewModelBase
    {
        public SanfordLib slb;
        //private OutputDevice _outputDevice;
        private bool _running;


        // This sequencer creates a possibility to play a sequence.
        // It has a timer and raises events on the right moments.
        //private Sequencer _sequencer;

        //public Sequence MidiSequence
        //{
        //    get { return slb._sequencer.Sequence; }
        //    set
        //    {
        //        StopCommand.Execute(null);
        //        slb._sequencer.Sequence = value;
        //        UpdateButtons();
        //    }
        //}

        public MidiPlayerViewModel(MusicLoader musicLoader)
        {
            // The OutputDevice is a midi device on the midi channel of your computer.
            // The audio will be streamed towards this output.
            // DeviceID 0 is your computer's audio channel.
            //_outputDevice = new OutputDevice(0);
            //_sequencer = new Sequencer();

            slb = new SanfordLib(PlayCommand, UpdateButtons);
            slb.SequencerChannelMessagedPlayed(slb.ChannelMessagePlayed);

            // Wanneer de sequence klaar is moeten we alles closen en stoppen.
            slb.SquencePlayCompleted(_running);
            
            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer?
            // musicLoader.MidiPlayerViewModel = this;
        }

        private void UpdateButtons()
        {
            PlayCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        // Wanneer een channelmessage langskomt sturen we deze direct door naar onze audio.
        // Channelmessages zijn tonen met commands als NoteOn en NoteOff
        // In midi wordt elke noot gespeeld totdat NoteOff is benoemd. Wanneer dus nooit een NoteOff komt nadat die een NoteOn heeft gehad
        // zal deze note dus oneindig lang blijven spelen.


        #region buttons for play, stop, pause
        public RelayCommand PlayCommand => new RelayCommand(() =>
        {
            if (!_running)
            {
                _running = true;
                slb.ContinueSequence();
                UpdateButtons();
            }
        });/*, () => !_running && slb.CheckSequence() == true);*/

        public RelayCommand StopCommand => new RelayCommand(() =>
        {
            _running = false;
            slb.SequencerStop();
            slb.SetSequncerPosition(0);
            UpdateButtons();
        }, () => _running);

        public RelayCommand PauseCommand => new RelayCommand(() =>
        {
            _running = false;
            slb.SequencerStop();
            UpdateButtons();
        }, () => _running);

        #endregion buttons for play, stop, pause

        /// <summary>
        /// Stop the player and clear the sequence on cleanup.
        /// </summary>
        public override void Cleanup()
        {
            slb.Cleanup();
            base.Cleanup();
        }
    }
}
