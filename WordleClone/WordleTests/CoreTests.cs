using NUnit.Framework;
using WordleCore;
using System;
using System.Linq;

namespace WordleTests
{
    public class CoreTests
    {
        [Test]
        public void TestWordleGame_BasicWin()
        {
            var game = new WordleGame("HELLO");
            var result = game.SubmitGuess("HELLO");

            Assert.That(result.IsValid, Is.True);
            Assert.That(game.Status, Is.EqualTo(GameStatus.Won));
            Assert.That(result.LetterResults!.All(l => l.State == LetterState.Correct), Is.True);
        }

        [Test]
        public void TestWordleGame_Feedback()
        {
            // Target: A P P L E
            // Guess:  A L P H A

            var game = new WordleGame("APPLE");
            var result = game.SubmitGuess("ALPHA");

            Assert.That(result.LetterResults![0].State, Is.EqualTo(LetterState.Correct)); // A
            Assert.That(result.LetterResults[1].State, Is.EqualTo(LetterState.WrongPosition)); // L
            Assert.That(result.LetterResults[2].State, Is.EqualTo(LetterState.Correct)); // P
            Assert.That(result.LetterResults[3].State, Is.EqualTo(LetterState.Absent)); // H
            Assert.That(result.LetterResults[4].State, Is.EqualTo(LetterState.Absent)); // A
        }

        [Test]
        public void TestWordGenerator_DailyConsistency()
        {
            var generator = new WordGenerator();
            var date = new DateTime(2023, 10, 27);

            string word1 = generator.GetDailyWord(date);
            string word2 = generator.GetDailyWord(date);

            Assert.That(word1, Is.EqualTo(word2));

            string wordNextDay = generator.GetDailyWord(date.AddDays(1));
            Assert.That(wordNextDay, Is.Not.Null);
        }

        [Test]
        public void TestStatisticsManager_StreakLogic()
        {
            var stats = new StatisticsManager();
            var today = DateTime.Today;

            // Day 1: Win
            stats.RecordGame(true, 4, today.AddDays(-2));
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(1));
            Assert.That(stats.Stats.GamesPlayed, Is.EqualTo(1));

            // Day 2: Win (Consecutive)
            stats.RecordGame(true, 3, today.AddDays(-1));
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(2));

            // Day 3: Win (Consecutive)
            stats.RecordGame(true, 5, today);
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(3));
            Assert.That(stats.Stats.GamesWon, Is.EqualTo(3));

            // Day 5: Win (Skipped Day 4)
            stats.RecordGame(true, 2, today.AddDays(2));
            // Should reset to 1
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(1));
        }

        [Test]
        public void TestStatisticsManager_LossResetsStreak()
        {
            var stats = new StatisticsManager();
            var today = DateTime.Today;

            stats.RecordGame(true, 3, today.AddDays(-1));
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(1));

            stats.RecordGame(false, 6, today);
            Assert.That(stats.Stats.CurrentStreak, Is.EqualTo(0));
        }
    }
}
