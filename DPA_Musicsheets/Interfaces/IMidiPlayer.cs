using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Interfaces
{
    public interface IMusicPlayer
    {
        void SetMusic(Symbol symbol);
        bool Check();
        void Rewind();
        void Stop();
        void Continue();
        void SquencePlayCompleted(bool running);
        void Cleanup();
    }
}
