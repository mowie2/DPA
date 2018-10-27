using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Memento;
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
        
        private MusicController musicController;
        private Editor editor;
        private string _text;


        private DPA_Musicsheets.Memento.CareTaker careTaker;
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
                  
                }
                _text = value;
                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private  DateTime _lastChange;
        private static readonly int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private  bool _waitingForRender = false;
        private LilyToDomain lilyToDomain;
        private bool ShouldCreateMemento = false;
        public LilypondViewModel(MusicController msc, Editor edit)
        {

            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            editor = edit;
            _text = "Your lilypond text will appear here.";
            musicController = msc;
            lilyToDomain = new LilyToDomain();
            careTaker = new CareTaker();
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

               

                Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
                {
                    if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                    {
                        _waitingForRender = false;
                        UndoCommand.RaiseCanExecuteChanged();
                        
                        
                        musicController.musicData = lilyToDomain.getRoot(LilypondText);
                        LilypondText = editor.TextChanged(musicController.musicData);
                        musicController.SetStaffs();
                        musicController.SetMidiPlayer();
                        

                        CreateMemento();
                        ShouldCreateMemento = true;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });


        private void CreateMemento()
        {
            if (ShouldCreateMemento)
            {
                DPA_Musicsheets.Memento.Memento memento = new DPA_Musicsheets.Memento.Memento(LilypondText);
                careTaker.AddMemento(memento);
                
            }
        }
        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            careTaker.Undo();
            LilypondText = careTaker.GetCurrentMemento().Text;
            ShouldCreateMemento = false;
        }, () => careTaker.canUndo);

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            careTaker.Redo();
            LilypondText = careTaker.GetCurrentMemento().Text;
            RedoCommand.RaiseCanExecuteChanged();
            ShouldCreateMemento = false;
        }, () => careTaker.canRedo);

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            musicController.Save();
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
