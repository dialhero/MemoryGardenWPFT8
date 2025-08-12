using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using MemoryGardenWPF.Models;
using MemoryGardenWPF.Services;
using MemoryGardenWPF.Utils;

namespace MemoryGardenWPF.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly Action _goToMenu;
        private readonly Random _rng = new();
        private DispatcherTimer? _countdownTimer;
        private DispatcherTimer? _gameTimer;
        private bool _busy;

        // Settings
        private ThemeKey _theme = ThemeKey.Animals;
        private BoardSize _boardSize = BoardSize.Easy;
        private bool _sfxOn = false;

        // State
        public ObservableCollection<CardViewModel> Deck { get; } = new();
        private int _rows, _cols, _pairsTarget;
        private int _moves, _foundPairs, _countdown; // seconds
        private bool _isPreviewing, _won;
        private int _peekLeft = 3;
        private TimeSpan _elapsed = TimeSpan.Zero;

        // HUD
        public int Rows { get => _rows; private set => SetProperty(ref _rows, value); }
        public int Cols { get => _cols; private set => SetProperty(ref _cols, value); }
        public int PairsTarget { get => _pairsTarget; private set => SetProperty(ref _pairsTarget, value); }
        public int Moves { get => _moves; private set => SetProperty(ref _moves, value); }
        public int FoundPairs { get => _foundPairs; private set => SetProperty(ref _foundPairs, value); }
        public int Countdown { get => _countdown; private set => SetProperty(ref _countdown, value); }
        public bool IsPreviewing { get => _isPreviewing; private set => SetProperty(ref _isPreviewing, value); }
        public bool Won { get => _won; private set => SetProperty(ref _won, value); }
        public int PeekLeft { get => _peekLeft; private set => SetProperty(ref _peekLeft, value); }
        public string TimeText => $"{(int)_elapsed.TotalMinutes}:{_elapsed.Seconds:00}";

        public ICommand NewGameCommand { get; }
        public ICommand PeekCommand { get; }
        public ICommand BackToMenuCommand { get; }

        public GameViewModel(Action goToMenu)
        {
            _goToMenu = goToMenu;
            NewGameCommand = new RelayCommand(_ => NewGame());
            PeekCommand = new RelayCommand(async _ => await PeekAsync(), _ => CanPeek());
            BackToMenuCommand = new RelayCommand(() => _goToMenu());
        }

        public void Configure(ThemeKey theme, BoardSize size, bool sfxOn)
        {
            _theme = theme; _boardSize = size; _sfxOn = sfxOn;
            SoundService.Enabled = sfxOn;
        }

        public void NewGame()
        {
            BuildBoard(_boardSize, out var rows, out var cols, out var pairs);
            Rows = rows; Cols = cols; PairsTarget = pairs;

            // Build deck from theme
            var emojis = GetEmojis(_theme).Take(pairs).ToList();
            var cards = new List<Card>();
            int id = 1;
            foreach (var e in emojis)
            {
                cards.Add(new Card { Id = id++, Emoji = e });
                cards.Add(new Card { Id = id++, Emoji = e });
            }
            // Shuffle
            cards = cards.OrderBy(_ => _rng.Next()).ToList();

            Deck.Clear();
            foreach (var c in cards)
                Deck.Add(new CardViewModel(c, OnFlipRequested));

            // Reset state
            Moves = 0; FoundPairs = 0; Won = false; _busy = true; PeekLeft = 3; _elapsed = TimeSpan.Zero; OnPropertyChanged(nameof(TimeText));

            // Show all, countdown 3..2..1
            foreach (var cvm in Deck) cvm.IsFaceUp = true;
            IsPreviewing = true; Countdown = 3;
            _countdownTimer?.Stop();
            _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _countdownTimer.Tick += (s, e) =>
            {
                if (Countdown > 0)
                {
                    if (_sfxOn) SoundService.Tick();
                    Countdown--;
                }
                else
                {
                    _countdownTimer!.Stop();
                    foreach (var cvm in Deck) cvm.IsFaceUp = false;
                    IsPreviewing = false; _busy = false; StartGameTimer();
                }
            };
            _countdownTimer.Start();
        }

        private void StartGameTimer()
        {
            _gameTimer?.Stop();
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _gameTimer.Tick += (s, e) => { _elapsed = _elapsed.Add(TimeSpan.FromSeconds(1)); OnPropertyChanged(nameof(TimeText)); };
            _gameTimer.Start();
        }

        private readonly List<CardViewModel> _flipped = new();
        private async void OnFlipRequested(CardViewModel card)
        {
            if (_busy || IsPreviewing || Won) return;
            if (card.IsFaceUp || card.IsMatched) return;

            card.IsFaceUp = true;
            if (_sfxOn) SoundService.Flip();
            _flipped.Add(card);

            if (_flipped.Count == 2)
            {
                _busy = true;
                Moves++;
                var a = _flipped[0];
                var b = _flipped[1];
                if (a.Emoji == b.Emoji)
                {
                    await Task.Delay(200);
                    a.IsMatched = b.IsMatched = true;
                    _flipped.Clear();
                    FoundPairs++;
                    if (_sfxOn) SoundService.Match();
                    _busy = false;
                    if (FoundPairs == PairsTarget)
                    {
                        Won = true;
                        _gameTimer?.Stop();
                        if (_sfxOn) SoundService.Win();
                    }
                }
                else
                {
                    if (_sfxOn) SoundService.Miss();
                    await Task.Delay(650);
                    a.IsFaceUp = false; b.IsFaceUp = false; _flipped.Clear(); _busy = false;
                }
            }
        }

        private bool CanPeek() => PeekLeft > 0 && !_busy && !IsPreviewing && !Won;

        private async Task PeekAsync()
        {
            if (!CanPeek()) return;
            PeekLeft--;
            _busy = true;
            var remember = Deck.Where(c => c.IsFaceUp).ToList();
            foreach (var c in Deck) c.IsFaceUp = true;
            await Task.Delay(1200);
            foreach (var c in Deck) c.IsFaceUp = c.IsMatched || remember.Contains(c);
            _busy = false;
        }

        // Helpers
        private static void BuildBoard(BoardSize size, out int rows, out int cols, out int pairs)
        {
            switch (size) //Mulighed for at opskalere
            {
                case BoardSize.Tiny:   rows = 2; cols = 3; pairs = 3; break;
                case BoardSize.Easy:   rows = 3; cols = 4; pairs = 6; break;
                default:               rows = 4; cols = 4; pairs = 8; break;
            }
        }

        //Mulighed for at udvide temaer
        private static IEnumerable<string> GetEmojis(ThemeKey theme) => theme switch
        {
            ThemeKey.Animals => new[] { "🐶","🐱","🦊","🐻","🐼","🐨","🦁","🐯","🐷","🐸","🐵","🐮","🦄","🐤","🐙","🐞","🦉","🐔","🐬","🐢" },
            ThemeKey.Fruits  => new[] { "🍎","🍌","🍓","🍇","🍉","🍒","🍍","🥝","🍑","🍐","🍊","🥥","🥭","🍋","🍈","🫐","🍏","🍅","🥕","🍠" },
            _                => new[] { "⭐️","❤️","🔷","🔶","🔺","🔻","⬜️","⬛️","⚫️","⚪️","🟦","🟩","🟨","🟧","🟪","🟥","🟫","🔵","🟠","🟣" }
        };
    }
}
