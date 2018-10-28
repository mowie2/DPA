using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Commands
{
    public class TimeSig3Command : Icommand
    {
        public List<KeyEventArgs> PressedKeys { get; }
        string text;
        public TimeSig3Command(List<KeyEventArgs> pressedKeys, string text)
        {
            PressedKeys = pressedKeys;
            this.text = text;
        }
        public bool CanExecute()
        {
            if (PressedKeys.Count > 3) return false;

            if (PressedKeys[0].Key != Key.LeftAlt) return false;
            if (PressedKeys[1].Key != Key.T) return false;
            if (PressedKeys[2].Key != Key.D3) return false;
            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            text += "  \\time 3/4";
        }
    }
}
