using DPA_Musicsheets.Facade;
using DPA_Musicsheets.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.Managers
{
    public class Editor : ViewModelBase
    {
        private static readonly int TIMEOUT_TEXT_EDITOR = 1500;
        private string _text;
        // private string _currentState;
        private DateTime _lastChange;
        //public string CurrentState
        //{
        //    get
        //    {
        //        return _currentState;
        //    }
        //    set
        //    {
        //        _currentState = value;
        //        base.RaisePropertyChanged("CurrentState");
        //    }
        //}
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                //CurrentState = "";
                base.RaisePropertyChanged("Text");
            }

        }

        private MainViewModel mainViewModel;
        private readonly MusicController musicController;
        public Editor(MainViewModel mvm,  MusicController msc)
        {
            mainViewModel = mvm;
            musicController = msc;
            Text = "test";
        }

        public ICommand TextChangedCommand => new RelayCommand<TextChangedEventArgs>(async (args) =>
        {
            // If we were typing, we need to do things.

            Text = musicController.lilyPondText;
            //_waitingForRender = true;
            _lastChange = DateTime.Now;

            mainViewModel.CurrentState = "Rendering...";
            await SendWithDelay();


        });

        private async Task SendWithDelay()
        {
            await Task.Delay(TIMEOUT_TEXT_EDITOR);
            mainViewModel.CurrentState = "";
            musicController.SetStaffs();
        }
    }
}
