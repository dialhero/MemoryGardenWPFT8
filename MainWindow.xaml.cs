using System.Windows;
using MemoryGardenWPF.ViewModels;

namespace MemoryGardenWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
