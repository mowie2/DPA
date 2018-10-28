using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel
{
    public class Composite : MusicSymbol
    {
        private List<MusicSymbol> musicSymbols;

        Composite()
        {
            musicSymbols = new List<MusicSymbol>();
        }
        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}