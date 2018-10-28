using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Commands
{
    public class SaveCommand : Icommand
    {
        private MusicController musicController;
        public List<KeyEventArgs> PressedKeys { get; }
        public SaveCommand(MusicController musicController, List<KeyEventArgs> pressedKeys)
        {
            this.musicController = musicController;
            this.PressedKeys = pressedKeys;
        }
        public bool CanExecute()
        {
            if (PressedKeys.Count > 2) return false;

            if (PressedKeys[0].Key != Key.LeftCtrl) return false;
            if (PressedKeys[1].Key != Key.S) return false;

            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            musicController.Save();
        }
    }
}
