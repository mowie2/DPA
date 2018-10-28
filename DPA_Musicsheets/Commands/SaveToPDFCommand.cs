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
    public class SaveToPDFCommand : Icommand
    {
        private MusicController musicController;
        public List<KeyEventArgs> PressedKeys { get; }

        public SaveToPDFCommand()
        {

        }
        public bool CanExecute()
        {
            if (PressedKeys.Count > 3) return false;

            if (PressedKeys[0].Key != Key.LeftCtrl) return false;
            if (PressedKeys[1].Key != Key.S) return false;
            if (PressedKeys[1].Key != Key.P) return false;

            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            musicController.SaveToPDF();
        }
    }
}
