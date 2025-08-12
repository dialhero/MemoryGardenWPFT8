using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MemoryGardenWPF.ViewModels;

namespace MemoryGardenWPF.Services
{
    public class GameStatsService
    {
        private string PlayerName { get; set; }
        private string ResultTimeText { get; set; }
        private string ResultMoves { get; set; }
        private static readonly string BoardgameFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Data\mock_boardgames.txt");

        public GameStatsService(string _playerName, string TimeText, string Moves)
        {
            PlayerName = _playerName;
            TimeText = ResultTimeText;
            Moves = ResultMoves;
        }


        public string SaveResult(string _playerName, string TimeText, string Moves)
        {
            if(File.Exists($"{BoardgameFilePath}")) // Check if the file exists in the current directory
            { 
                try
                {
                    StreamWriter writer = new StreamWriter($"{BoardgameFilePath}", true);
                    writer.WriteLine($"{_playerName};{TimeText};{Moves}");
                    return MessageBox.Show("Game stats saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information).ToString();
                    writer.Close();
                }
                catch (Exception ex)
                {
                    return MessageBox.Show($"Error saving game stats: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error).ToString();
                }
            }
            else
            {
                try
                {
                    StreamWriter writer = new StreamWriter($"{BoardgameFilePath}");
                    writer.WriteLine($"{_playerName};{TimeText};{Moves}");
                    return MessageBox.Show("Game stats saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information).ToString();
                    writer.Close();
                }
                catch (Exception ex)
                {
                    return MessageBox.Show($"Error creating game stats file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error).ToString();
                }
            }
    }
}
