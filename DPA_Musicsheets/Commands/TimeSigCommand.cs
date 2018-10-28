using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Commands
{
    public class TimeSigCommand : Icommand
    {
        public List<KeyEventArgs> PressedKeys { get; }
        string text;
        public TimeSigCommand(List<KeyEventArgs> pressedKeys, string text)
        {
            PressedKeys = pressedKeys;
            this.text = text;
        }
        public bool CanExecute()
        {
            if (PressedKeys.Count > 2) return false;

            if (PressedKeys[0].Key != Key.LeftAlt) return false;
            if (PressedKeys[1].Key != Key.T) return false;

            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            text += "  \\time 4/4";
        }
    }
}
