using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Interfaces
{
    public interface IReader
    {
        Symbol readFile(string filename);
        string GetMusicText();
    }
}
