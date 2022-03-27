using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RepeatEnglish.Model;
using RepeatEnglish.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepeatEnglish
{
    public class WordCardDeck
    {
        // Array of word cards that make up the word card deck:
        private Word[] wordCards = new Word[0];

        public WordCardDeck()
        {
            List<Word> wordList = WordService.getWordsForChecking();
            if (wordList.Count > 0)
                this.wordCards = wordList.ToArray();
        }

        // Indexer (read only) for accessing a word card:
        public Word this[int i] { get { return wordCards[i]; } }

        // Returns the number of word cards in the deck:
        public int NumCards { get { return wordCards.Length; } }
    }


}