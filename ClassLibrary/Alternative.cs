using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Alternative
    {
        public List<Note> Notes { get; set; }
        public Alternative()
        {
            Notes = new List<Note>();
        }
    }
}
