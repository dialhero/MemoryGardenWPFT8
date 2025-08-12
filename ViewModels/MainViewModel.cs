using MemoryGardenWPF.Models;

namespace MemoryGardenWPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public object CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }
        private object _currentViewModel;

        public MenuViewModel MenuVM { get; }
        public GameViewModel GameVM { get; }

        public MainViewModel()
        {
            GameVM = new GameViewModel(GoToMenu);
            MenuVM = new MenuViewModel(StartGame);
            CurrentViewModel = MenuVM;
        }

        private void StartGame(ThemeKey theme, BoardSize size, bool sfxOn)
        {
            GameVM.Configure(theme, size, sfxOn);
            CurrentViewModel = GameVM;
            GameVM.NewGame();
        }

        private void GoToMenu()
        {
            CurrentViewModel = MenuVM;
        }
    }
}
