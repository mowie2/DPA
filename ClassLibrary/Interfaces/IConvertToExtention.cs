using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Interfaces
{
    public interface IConvertToExtention
    {
        object Convert(Symbol toConvert);
        string GetExtention();
    }
}
