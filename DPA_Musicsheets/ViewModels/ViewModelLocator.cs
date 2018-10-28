using DPA_Musicsheets.Facade;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace DPA_Musicsheets.ViewModels
{
    /// <summary>
    /// This is the MVVM-Light implementation of dependency injection.
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

     

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LilypondViewModel>();
            SimpleIoc.Default.Register<StaffsViewModel>();
            SimpleIoc.Default.Register<MidiPlayerViewModel>();

            SimpleIoc.Default.Register<MusicLoader>();
            SimpleIoc.Default.Register<MusicController>();
            SimpleIoc.Default.Register<PsamContolLib>();
            SimpleIoc.Default.Register<Editor>();
        }


        public LilypondViewModel LilypondViewModel => ServiceLocator.Current.GetInstance<LilypondViewModel>();
        public StaffsViewModel StaffsViewModel => ServiceLocator.Current.GetInstance<StaffsViewModel>();
        public MidiPlayerViewModel MidiPlayerViewModel => ServiceLocator.Current.GetInstance<MidiPlayerViewModel>();
        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        public Editor Editor => ServiceLocator.Current.GetInstance<Editor>();

        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MidiPlayerViewModel>().Cleanup();
        }
    }
}