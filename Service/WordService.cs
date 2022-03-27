using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RepeatEnglish.Model;
using RepeatEnglish.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.OS.Environment;

namespace RepeatEnglish.Service
{
    public static class WordService
    {
        public static String insertWord(string wordOriginal, string wordTranslated)
        {
            String result = "";
            Word word = WordRepository.getWordByWordOriginal(wordOriginal);
            if (word != null)
            {
                word.DateUpdated = DateTime.Now;
                word.AddCounter = ++word.AddCounter;
                word.Rating = getRatingValue(word);
                if (WordRepository.updateWord(word))
                {
                    result = "Update word successfully";
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                Word newWord = new Word(wordOriginal, wordTranslated, now, now, null, 0, 0, 0, 0);
                newWord.Rating = getRatingValue(newWord);
                if (WordRepository.insertWord(newWord))
                {
                    result = "Insert word successfully:";
                }
            }
            return result;
        }

        public static List<Word> getWordsForChecking()
        {
            int wordCount = Preferences.Get(Const.SETTING_WORD_CHECKING_COUNT, 10);
            List<Word> wordList = WordRepository.getWordsForChecking(wordCount);
            return wordList;
        }

        public static Boolean incrementCorrectCheckCounter(String wordOriginal)
        {
            Word word = WordRepository.getWordByWordOriginal(wordOriginal);
            if (word != null)
            {
                word.CorrectCheckCounter += 1;
                word.Rating = getRatingValue(word);
                if (WordRepository.incrementWordCorrectCheckCounter(word))
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean incrementIncorrectCheckCounter(String wordOriginal)
        {
            Word word = WordRepository.getWordByWordOriginal(wordOriginal);
            if (word != null)
            {
                word.IncorrectCheckCounter += 1;
                word.Rating = getRatingValue(word);
                if (WordRepository.incrementWordIncorrectCheckCounter(word))
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean updateDateShowed(String wordOriginal)
        {
            Word word = WordRepository.getWordByWordOriginal(wordOriginal);
            if (word != null)
            {
                if (WordRepository.updateWordDateShowed(word))
                {
                    return true;
                }
            }
            return false;
        }

        private static long getRatingValue(Word word)
        {
            long result = 1000;
            long k1 = word.AddCounter;
            long k2 = (DateTime.Now.Year - word.DateUpdated.Year) * 12 + DateTime.Now.Month - word.DateUpdated.Month;
            long k3 = word.CorrectCheckCounter;
            long k4 = word.IncorrectCheckCounter;
            long k5 = word.WordOriginal.Length;

            result = result + k1 - k2 - k3 + k4 + k5;
            return result;
        }

        public static async Task<String> exportDbInFile()
        {
            if (!MediaMounted.Equals(ExternalStorageState))
            {
                return null;
            }

            List<Word> wordList = WordRepository.getWordsForExport();
            if (wordList.Count == 0)
            {
                return null;
            }

            String fileName = String.Format("repeatEnglish_{0}_{1}_{2}_{3}_{4}_{5}.csv"
                  , DateTime.Now.Day
                  , DateTime.Now.Month
                  , DateTime.Now.Year
                  , DateTime.Now.Hour
                  , DateTime.Now.Minute
                  , DateTime.Now.Second);
            String filePath = Path.Combine(Application.Context.GetExternalFilesDir(null).AbsolutePath,
                fileName);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                await sw.WriteLineAsync("Id,WordOriginal,WordTranslated,DateCreated,DateUpdated,DateShowed,AddCounter,CorrectCheckCounter,IncorrectCheckCounter,Rating");
                foreach (Word word in wordList)
                {
                    await sw.WriteLineAsync(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
                        , word.Id
                        , word.WordOriginal
                        , word.WordTranslated
                        , word.DateCreated.Ticks
                        , word.DateUpdated.Ticks
                        , word.DateShowed.HasValue ? word.DateShowed.Value.Ticks : 0
                        , word.AddCounter
                        , word.CorrectCheckCounter
                        , word.IncorrectCheckCounter
                        , word.Rating));
                }
            }
            return filePath;
        }

        public static async Task<String> importFileInDb(String filePath)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                return null;
            }
            WordRepository.clearWordTable();
            using (var reader = new StreamReader(filePath, true))
            {
                string line = await reader.ReadLineAsync();
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] wordArr = line.Split(",", 10, StringSplitOptions.None);
                    Word word = null;
                    for (int i = 0; i < wordArr.Length; ++i)
                    {
                        String wordOriginal = wordArr[1];
                        String wordTranslated = wordArr[2];
                        DateTime dateCreated = new DateTime(long.Parse(wordArr[3]));
                        DateTime dateUpdated = new DateTime(long.Parse(wordArr[4]));
                        DateTime? dateShowed = null;
                        long dateShowedLong = long.Parse(wordArr[5]);
                        if (dateShowedLong > 0)
                        {
                            dateShowed = new DateTime(dateShowedLong);
                        }
                        long addCounter = long.Parse(wordArr[6]);
                        long correctCheckCounter = long.Parse(wordArr[7]);
                        long incorrectCheckCounter = long.Parse(wordArr[8]);
                        long rating = long.Parse(wordArr[9]);

                        word = new Word(wordOriginal, wordTranslated, dateCreated, dateUpdated, dateShowed,
                            addCounter, correctCheckCounter, incorrectCheckCounter, rating);
                    }
                    if (word != null)
                    {
                        WordRepository.insertWord(word);
                    }
                }
            }
            return filePath;
        }
    }

}