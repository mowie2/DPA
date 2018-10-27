using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DPA_Musicsheets.Interfaces;
namespace DPA_Musicsheets.Commands
{
    class ClefCommand : Icommand
    {
        public List<KeyEventArgs> PressedKeys { get; }
        string text;

        public ClefCommand(List<KeyEventArgs> pressedKeys, string text)
        {
            this.PressedKeys = pressedKeys;
            this.text = text;
        }


        public bool CanExecute()
        {
            if (PressedKeys.Count > 2) return false;

            if (PressedKeys[0].Key != Key.LeftShift) return false;
            if (PressedKeys[1].Key != Key.C) return false;

            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            text += "\\clef treble";
        }
    }
}
