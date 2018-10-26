using ClassLibrary;
using DPA_Musicsheets.Converters;
using DPA_Musicsheets.Facade;
using DPA_Musicsheets.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.Managers
{
    public class Editor : ViewModelBase
    {
        private DomainToLily converter;
        public Editor()
        {
            converter = new DomainToLily();
        }
        public string TextChanged()
        {
            return converter.GetLilyText(new Note());
        }
    }
}
