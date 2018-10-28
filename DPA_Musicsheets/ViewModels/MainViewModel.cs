using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
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
        private DateTime _lastChange;
        List<KeyEventArgs> pressedKeys;
        public MainViewModel(MusicController ms, LilypondViewModel lvm)
        {

            pressedKeys = new List<KeyEventArgs>();
            musicController = ms;
            lilypondViewModel = lvm;
            FileName = "";

            //CurrentState = this.ed.CurrentState;
        }

        public ICommand OpenFileCommand => new RelayCommand(() =>
        {
            FileName = musicController.OpenFile();
        });

        public ICommand LoadCommand => new RelayCommand(() =>
        {   
            if(FileName == "")
            {
                MessageBox.Show("Please select a file");
                return;
            }
            musicController.LoadFile();
            lilypondViewModel.SetLilyText();
        });

        #region Focus and key commands, these can be used for implementing hotkeys
        public ICommand OnLostFocusCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Maingrid Lost focus");
        });

        
        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) =>
        {
            Console.WriteLine($"Key down: {e.Key}");
            ShortCutes(e);
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

        private static int MILLISECONDS_BEFORE_CHANGE_HANDLED = 500;
        private void ShortCutes(KeyEventArgs key)
        {
            _lastChange = DateTime.Now;

            pressedKeys.Add(key);

            
            Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
            {
                if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                {

                    lilypondViewModel.InsertKeys(pressedKeys);
                    pressedKeys.Clear();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
        }
    }
}
