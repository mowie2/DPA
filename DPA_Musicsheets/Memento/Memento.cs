using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Memento
{
    class Memento
    {
        public string Text{ get; }
        public Memento Previous { get; set; }
        public Memento Next { get; set; }
        public Memento(string text)
        {
            Text = text;
        }
    }
}
