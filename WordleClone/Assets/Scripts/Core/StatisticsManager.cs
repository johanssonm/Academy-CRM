using System;
using System.Collections.Generic;

namespace WordleCore
{
    [Serializable]
    public class PlayerStats
    {
        public int GamesPlayed;
        public int GamesWon;
        public int CurrentStreak;
        public int MaxStreak;
        public int[] GuessDistribution; // Index 0 = 1 guess, Index 5 = 6 guesses
        public DateTime LastPlayedDate;

        public PlayerStats()
        {
            GamesPlayed = 0;
            GamesWon = 0;
            CurrentStreak = 0;
            MaxStreak = 0;
            GuessDistribution = new int[6];
            LastPlayedDate = DateTime.MinValue;
        }
    }

    public class StatisticsManager
    {
        public PlayerStats Stats { get; private set; }

        public StatisticsManager(PlayerStats? stats = null)
        {
            Stats = stats ?? new PlayerStats();
        }

        public void RecordGame(bool won, int attempts, DateTime date)
        {
            // If the game was already played today, we generally shouldn't affect streaks again,
            // but for simplicity we assume the caller enforces "One Daily Game".

            // However, if we support practice games, we might need a flag.
            // Let's assume this method is called for the Daily Challenge.

            Stats.GamesPlayed++;

            if (won)
            {
                Stats.GamesWon++;

                if (Stats.LastPlayedDate.Date == date.Date.AddDays(-1))
                {
                    // Played yesterday, streak continues
                    Stats.CurrentStreak++;
                }
                else if (Stats.LastPlayedDate.Date == date.Date)
                {
                    // Played today already? If we are recording another win today,
                    // we probably shouldn't increment streak again if it's the same daily.
                    // But if it's a new "random" game, maybe we do?
                    // Let's assume for Daily Wordle, this doesn't happen often.
                    // We'll leave streak as is if played today.
                }
                else
                {
                    // Missed a day (or more), or first time playing
                    Stats.CurrentStreak = 1;
                }

                if (Stats.CurrentStreak > Stats.MaxStreak)
                {
                    Stats.MaxStreak = Stats.CurrentStreak;
                }

                if (attempts >= 1 && attempts <= 6)
                {
                    Stats.GuessDistribution[attempts - 1]++;
                }
            }
            else
            {
                // Lost the game, streak resets
                Stats.CurrentStreak = 0;
            }

            Stats.LastPlayedDate = date;
        }
    }
}
