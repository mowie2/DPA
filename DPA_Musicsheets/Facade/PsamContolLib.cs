using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using DPA_Musicsheets.Models;
using PSAMControlLibrary;

namespace DPA_Musicsheets.Facade
{
    public class PsamContolLib
    {
        public List<MusicalSymbol> WPFStaffs { get; set; }
        
        public PsamContolLib()
        {
            WPFStaffs = new List<MusicalSymbol>();            
        }

        public IEnumerable<MusicalSymbol> GetStaffsFromTokens(Symbol rootNote)
        {
            List<MusicalSymbol> symbols = new List<MusicalSymbol>();
            Symbol currentNote = rootNote;

            while(currentNote != null)
            {



                currentNote = currentNote.nextSymbol;
            }

            return symbols;
        }
    }
}
