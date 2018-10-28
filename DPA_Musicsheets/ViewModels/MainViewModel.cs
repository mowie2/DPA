using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        /// <summary>
        /// The current state can be used to display some text.
        /// "Rendering..." is a text that will be displayed for example.
        /// </summary>
        private string _currentState;
        public string CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; RaisePropertyChanged(() => CurrentState); }
        }

        private MusicController musicController;
        private LilypondViewModel lilypondViewModel;
        
        public MainViewModel(MusicController ms, LilypondViewModel lvm)
        {

            musicController = ms;
            lilypondViewModel = lvm;
            FileName = @"Files/Alle-eendjes-zwemmen-in-het-water.mid";

            //CurrentState = this.ed.CurrentState;
        }

        public ICommand OpenFileCommand => new RelayCommand(() =>
        {
            //FileManager fr = new FileManager();
            //FileName = fr.OpenFile();

            musicController.OpenFile();
        });

        public ICommand LoadCommand => new RelayCommand(() =>
        {
             
            lilypondViewModel.LilypondText = musicController.LoadFile();
        });

        #region Focus and key commands, these can be used for implementing hotkeys
        public ICommand OnLostFocusCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Maingrid Lost focus");
        });

        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) =>
        {
            Console.WriteLine($"Key down: {e.Key}");
        });

        public ICommand OnKeyUpCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Key Up");
        });

        public ICommand OnWindowClosingCommand => new RelayCommand(() =>
        {
            ViewModelLocator.Cleanup();
        });
        #endregion Focus and key commands, these can be used for implementing hotkeys
    }
}
