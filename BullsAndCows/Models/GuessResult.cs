using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndCows.Models
{
    public class GuessResult
    {

        public int Bulls { get; set; }
        public int Cows { get; set; }
        public bool IsWinningGuess
        {
            get { return Bulls == 4; }
        }


    }
}
