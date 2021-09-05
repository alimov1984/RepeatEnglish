using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;

namespace RepeatEnglish
{
    public class WordCardDeckAdapter : FragmentPagerAdapter
    {
        // Underlying model data (word card deck):
        public WordCardDeck wordCardDeck;

        // Constructor accepts a deck of flash cards:
        public WordCardDeckAdapter(Android.Support.V4.App.FragmentManager fm, WordCardDeck wordCards)
            : base(fm)
        {
            this.wordCardDeck = wordCards;
        }

        // Returns the number of cards in the deck:
        public override int Count { get { return wordCardDeck.NumCards; } }

        // Returns a new fragment for the flash card at this position:
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return WordCardFragment.newInstance(wordCardDeck[position].WordOriginal, wordCardDeck[position].WordTranslated);
        }

        // Display the word number in the PagerTitleStrip:
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("Word " + (position + 1));
        }

    }

}