﻿using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {
        private MusicLoader _musicLoader;
        private MainViewModel _mainViewModel { get; set; }
        private MusicController musicController;
        private Editor editor;
        private string _text;
        private string _previousText;
        private string _nextText;

        /// <summary>
        /// This text will be in the textbox.
        /// It can be filled either by typing or loading a file so we only want to set previoustext when it's caused by typing.
        /// </summary>
        public string LilypondText
        {
            get
            {
                return _text;
            }
            set
            {
                if (!_waitingForRender && !_textChangedByLoad)
                {
                    _previousText = _text;
                }
                _text = value;
                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private  DateTime _lastChange;
        private static readonly int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private  bool _waitingForRender = false;
        //private LilyToDomain lilyToDomain;
        public LilypondViewModel(MainViewModel mainViewModel, MusicLoader musicLoader, MusicController msc, Editor edit)
        {

            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            _mainViewModel = mainViewModel;
            _musicLoader = musicLoader;
            _musicLoader.LilypondViewModel = this;
            editor = edit;
            _text = "Your lilypond text will appear here.";
            musicController = msc;
            //lilyToDomain = new LilyToDomain();
        }

        public void LilypondTextLoaded(string text)
        {
            _textChangedByLoad = true;
            LilypondText = _previousText = text;
            _textChangedByLoad = false;
        }
 
        /// <summary>
        /// This occurs when the text in the textbox has changed. This can either be by loading or typing.
        /// </summary>
        public ICommand TextChangedCommand => new RelayCommand<TextChangedEventArgs>((args) =>
        {
            
            // If we were typing, we need to do things.
            if (!_textChangedByLoad)
            {
                _waitingForRender = true;
                _lastChange = DateTime.Now;

                _mainViewModel.CurrentState = "Rendering...";

                Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
                {
                    if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                    {
                        _waitingForRender = false;
                        UndoCommand.RaiseCanExecuteChanged();

                        //_musicLoader.LoadLilypondIntoWpfStaffsAndMidi(LilypondText);
                        //LilypondText = editor.TextChanged(lilyToDomain.getRoot(LilypondText));
                        //musicController.SetStaffs(lilyToDomain.getRoot(LilypondText));
                        _mainViewModel.CurrentState = "";
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });

        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            _nextText = LilypondText;
            LilypondText = _previousText;
            _previousText = null;
        }, () => _previousText != null && _previousText != LilypondText);

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            _previousText = LilypondText;
            LilypondText = _nextText;
            _nextText = null;
            RedoCommand.RaiseCanExecuteChanged();
        }, () => _nextText != null && _nextText != LilypondText);

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            musicController.Save();
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
