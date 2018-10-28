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
        private MusicController musicController;
        private bool _running;



        

        public MidiPlayerViewModel(MusicController musicController)
        {
            this.musicController = musicController;
            // The OutputDevice is a midi device on the midi channel of your computer.
            // The audio will be streamed towards this output.
            // DeviceID 0 is your computer's audio channel.
            
            // Wanneer de sequence klaar is moeten we alles closen en stoppen.
            musicController.musicPlayer.SquencePlayCompleted(_running);
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
            musicController.musicPlayer.SquencePlayCompleted(_running);
            if (!_running)
            {
                _running = true;
                musicController.Play();
                UpdateButtons();
            }
        }, () => !_running && musicController.musicPlayer.Check() == true);

        public RelayCommand StopCommand => new RelayCommand(() =>
        {
            _running = false;
            musicController.musicPlayer.Stop();
            musicController.musicPlayer.Rewind();
            UpdateButtons();
        }, () => _running);

        public RelayCommand PauseCommand => new RelayCommand(() =>
        {
            _running = false;
            musicController.musicPlayer.Stop();
            UpdateButtons();
        }, () => _running);

        #endregion buttons for play, stop, pause

        /// <summary>
        /// Stop the player and clear the sequence on cleanup.
        /// </summary>
        public override void Cleanup()
        {
            musicController.musicPlayer.Cleanup();
            base.Cleanup();
        }
    }
}
