using System.Windows.Input;
using MemoryGardenWPF.Models;
using MemoryGardenWPF.Utils;

namespace MemoryGardenWPF.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly Action<ThemeKey, BoardSize, bool> _startGame;
        private ThemeKey _selectedTheme = ThemeKey.Animals;
        private BoardSize _selectedSize = BoardSize.Easy;
        private bool _sfxOn = false; //Sat til false for ikke at have lyd på som default

        public ThemeKey SelectedTheme { get => _selectedTheme; set => SetProperty(ref _selectedTheme, value); }
        public BoardSize SelectedSize { get => _selectedSize; set => SetProperty(ref _selectedSize, value); }
        public bool SfxOn { get => _sfxOn; set => SetProperty(ref _sfxOn, value); }

        public ICommand StartGameCommand { get; }

        public MenuViewModel(Action<ThemeKey, BoardSize, bool> startGame)
        {
            _startGame = startGame;
            StartGameCommand = new RelayCommand(() => _startGame(SelectedTheme, SelectedSize, SfxOn));
        }
    }
}
