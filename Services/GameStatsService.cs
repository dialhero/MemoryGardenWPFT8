using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGardenWPF.Services
{
    public class GameStatsService
    {
        private string PlayerName { get; set; }
        private string Clicks {  get; set; }
        private string TimeElapsed { get; set; }

        public GameStatsService(string _playerName, string _clicks, string _timeElapsed)
        {
            PlayerName = _playerName;
            Clicks = _clicks;
            TimeElapsed = _timeElapsed; 

        }



        public string SaveResult()
            { 
                
            }


    }
}
