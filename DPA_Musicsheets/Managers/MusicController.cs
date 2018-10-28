using DomainModel;
using DPA_Musicsheet;
using DPA_Musicsheets.Facade;
using DPA_Musicsheets.Interfaces;
using DPA_Musicsheets.ViewModels;
using GalaSoft.MvvmLight;
using LilypondAdapter;

namespace DPA_Musicsheets.Managers
{
    public class MusicController : ViewModelBase
    {
        FileManager fileManager;
        public IMusicPlayer musicPlayer;
  
        string path;
        public Symbol musicData;
        private PsamContolLib psamContolLib;
        private Editor editor;

        private StaffsViewModel staffsViewModel;
        public MusicController(StaffsViewModel staffs, Editor edit)
        {

            fileManager = new FileManager();
            psamContolLib = new PsamContolLib();
            staffsViewModel = staffs;
            editor = edit;

            musicPlayer = new SanfordLib();
        }

        public void SetMusicPlayer()
        {
            musicPlayer.SetMusic(musicData);
        }

        public void Play()
        {
            musicPlayer.Continue();
        }

        public void Save()
        {
            fileManager.SaveFile(musicData);
        }

        public void SaveToPDF()
        {
            fileManager.SaveFile(musicData, ".pdf");
        }

        public string OpenFile()
        {
            path = fileManager.OpenFile();
            return path;
        }

        public void LoadFile()
        {
            musicData = fileManager.LoadFile(path);
            SetMusicPlayer();
            SetStaffs(musicData);
        }
        public void SetStaffs(Symbol symbol)
        {
            staffsViewModel.SetStaffs(psamContolLib.GetStaffsFromTokens(symbol));
        }
    }
}
