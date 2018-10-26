using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class NullClef : AbstractClef
    {
        public NullClef()
        {
            key = Key.G;
        }

        public override void GetSymbol()
        {
            throw new NotImplementedException();
        }

        public override bool isNil()
        {
            return true;
        }
    }
}
