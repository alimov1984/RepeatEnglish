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
using System.Linq;
using System.Text;

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
                word.UpdateCounter = ++word.UpdateCounter;
                if (WordRepository.updateWord(word))
                {
                    result = "Update word successfully";
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                Word newWord = new Word(wordOriginal, wordTranslated, now, now, 0, 0, 0);
                if (WordRepository.insertWord(newWord))
                {
                    result = "Insert word successfully:";
                }
            }
            return result;
        }

        public static List<Word> getWordsForChecking()
        {
            List<Word> wordList = WordRepository.getWordsForChecking();
            return wordList;
        }
    }
}