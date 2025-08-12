using MemoryGardenWPF.Models;
using MemoryGardenWPF.Utils;

namespace MemoryGardenWPF.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private readonly Action<CardViewModel> _onFlipRequested;
        private bool _isFaceUp;
        private bool _isMatched;

        public int Id { get; }
        public string Emoji { get; }

        public bool IsFaceUp
        {
            get => _isFaceUp;
            set => SetProperty(ref _isFaceUp, value);
        }
        public bool IsMatched
        {
            get => _isMatched;
            set => SetProperty(ref _isMatched, value);
        }

        public RelayCommand FlipCommand { get; }

        public CardViewModel(Card card, Action<CardViewModel> onFlip)
        {
            Id = card.Id;
            Emoji = card.Emoji;
            IsMatched = card.IsMatched;
            _onFlipRequested = onFlip;
            FlipCommand = new RelayCommand(() => _onFlipRequested(this));
        }
    }
}
