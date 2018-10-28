using DPA_Musicsheets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Commands
{
    public class TempoCommand : Icommand                            
    {
        public List<KeyEventArgs> PressedKeys { get; }
        string text;
        public TempoCommand(List<KeyEventArgs> pressedKeys, string text)
        {
            PressedKeys = pressedKeys;
            this.text = text;
        }
        public bool CanExecute()
        {
            if (PressedKeys.Count > 2) return false;

            if (PressedKeys[0].Key != Key.LeftAlt) return false;
            if (PressedKeys[1].Key != Key.S) return false;

            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            text += "  \\tempo 4=120";
        }
    }
}
