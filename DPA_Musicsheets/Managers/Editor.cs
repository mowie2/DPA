using DomainModel;
using GalaSoft.MvvmLight;

namespace DPA_Musicsheets.Managers
{
    public class Editor : ViewModelBase
    {
        //private DomainToLily converter;
        public Editor()
        {
            //converter = new DomainToLily();
        }
        public string TextChanged(Symbol symbol)
        {
            return "test"; 
            //return converter.GetLilyText(symbol);
        }
    }
}
