using System;
using System.Collections.Generic;

namespace WordleCore
{
    public class WordGenerator
    {
        private readonly List<string> _words;

        public WordGenerator()
        {
            // A small sample list of 5-letter words
            _words = new List<string>
            {
                "APPLE", "BEACH", "BRAIN", "BREAD", "BRUSH",
                "CHAIR", "CHEST", "CHORD", "CLICK", "CLOCK",
                "CLOUD", "DANCE", "DIARY", "DRINK", "DRIVE",
                "EARTH", "FEAST", "FIELD", "FRUIT", "GLASS",
                "GRAPE", "GREEN", "GHOST", "HEART", "HOUSE",
                "JUICE", "LIGHT", "LEMON", "MELON", "MONEY",
                "MUSIC", "NIGHT", "PARTY", "PIANO", "PILOT",
                "PHONE", "PLANE", "PLATE", "RADIO", "RIVER",
                "ROBOT", "SHIRT", "SHOES", "SMILE", "SNAKE",
                "SPACE", "SPOON", "STORM", "TABLE", "TIGER",
                "TOAST", "TOUCH", "TRAIN", "TRUCK", "VOICE",
                "WATCH", "WATER", "WHALE", "WORLD", "WRITE",
                "YOUTH", "ZEBRA"
            };
        }

        public string GetDailyWord(DateTime date)
        {
            // Use the date to seed a Random.
            // This ensures that for a specific date, the result is always the same.
            // We use the Year, Month, and Day to create the seed.

            int seed = date.Year * 10000 + date.Month * 100 + date.Day;
            var random = new Random(seed);

            int index = random.Next(_words.Count);
            return _words[index];
        }

        public string GetRandomWord()
        {
            var random = new Random();
            int index = random.Next(_words.Count);
            return _words[index];
        }
    }
}
