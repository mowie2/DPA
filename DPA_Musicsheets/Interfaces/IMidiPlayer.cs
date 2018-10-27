using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Interfaces
{
    interface IMidiPlayer
    {
        void SetMidisequence(Symbol symbol);
        bool CheckSequence();
        void SetSequncerPosition(int position);
        void SequencerStop();
        void ContinueSequence();
    }
}
