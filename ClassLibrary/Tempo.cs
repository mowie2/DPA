﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Tempo : Symbol
    {
        public int noteDuration { get; set; }
        public int bpm { get; set; }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }
    }
}
