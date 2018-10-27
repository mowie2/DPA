using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DPA_Musicsheets.Interfaces
{
    interface Icommand
    {
        void Execute();
        bool CanExecute();
    }
}
