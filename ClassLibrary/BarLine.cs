using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class BarLine : Symbol
    {

        public enum TYPE { FINAL, REPEAT, NORMAL, START }
        public BarLine Buddy { get; set; }
        private bool HasBeenPlayed;
        public TYPE Type { get; set; }
        public List<Note> Alternatives { get; set; }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
