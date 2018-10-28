﻿using ClassLibrary.Interfaces;
using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.Memento;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {

        private MusicController musicController;
        private IConvertToDomain converter;
        private Editor editor;
        private string _text;
        private List<Icommand> Commands;

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

        private readonly bool _textChangedByLoad = false;
        private DateTime _lastChange;
        private static readonly int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        
        private bool ShouldCreateMemento = false;
        private bool _waitingForRender = false;
        //private LilyToDomain lilyToDomain;
        public LilypondViewModel(MusicController msc, Editor edit)
        {

            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            editor = edit;
            _text = "Your lilypond text will appear here.";
            musicController = msc;
            careTaker = new CareTaker();
            Commands = new List<Icommand>();
            //lilyToDomain = new LilyToDomain();




            IEnumerable<Type> assemblies;
            var type = typeof(IConvertToDomain);
            var spath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            assemblies = Directory.GetFiles(spath, "*.dll")
                .Select(dll => Assembly.LoadFile(dll))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && p.IsPublic && !p.IsAbstract);

            var converters = assemblies.Where(p => type.IsAssignableFrom(p)).Select(c => (IConvertToDomain)Activator.CreateInstance(c)).ToList();
            converters = converters.Where(p => p.GetExtention().Equals(".ly")).ToList();

            if (converters.Count > 0)
            {
                converter = converters[0];
            }


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

                        musicController.musicData = converter.Convert(LilypondText);
                        LilypondText = editor.TextChanged(converter.Convert(LilypondText));
                        musicController.SetStaffs(converter.Convert(LilypondText));
                        musicController.SetMidiPlayer();
                        musicController.SetStaffs();

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

        public void InsertKeys(List<KeyEventArgs> pressedKeys)
        {
            PopulateCommands(pressedKeys);

            foreach (Icommand command in Commands)
            {
                command.Execute();
            }

            Commands.Clear();
        }

        private void PopulateCommands(List<KeyEventArgs> pressedKeys)
        {
            Commands.Add(new ClefCommand(pressedKeys, LilypondText));
            Commands.Add(new OpenCommand(musicController, pressedKeys));
            Commands.Add(new SaveCommand(musicController, pressedKeys));
            Commands.Add(new TempoCommand(pressedKeys, LilypondText));
            Commands.Add(new TimeSig3Command(pressedKeys, LilypondText));
            Commands.Add(new TimeSig4Command(pressedKeys, LilypondText));
            Commands.Add(new TimeSig6Command(pressedKeys, LilypondText));
            Commands.Add(new TimeSigCommand(pressedKeys, LilypondText));

        }
    }
}
