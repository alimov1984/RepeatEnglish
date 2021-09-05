using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepeatEnglish
{
    public class WordCardFragment : Android.Support.V4.App.Fragment
    {
        // Define Bundle keys for the word card question and answer
        private static string WORD_CARD_QUESTION = "card_question";
        private static string WORD_CARD_ANSWER = "card_answer";

        // Empty constructor: a factory method (below) is used instead.
        public WordCardFragment() { }

        // Static factory method that creates and initializes a new word card fragment:
        public static WordCardFragment newInstance(String question, String answer)
        {
            // Instantiate the fragment class:
            WordCardFragment fragment = new WordCardFragment();

            // Pass the question and answer to the fragment:
            Bundle args = new Bundle();
            args.PutString(WORD_CARD_QUESTION, question);
            args.PutString(WORD_CARD_ANSWER, answer);
            fragment.Arguments = args;

            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Get the question and answer for this word card fragment:
            string question = Arguments.GetString(WORD_CARD_QUESTION, "");
            string answer = Arguments.GetString(WORD_CARD_ANSWER, "");

            // Inflate this fragment from the "wordcard_layout":
            View view = inflater.Inflate(Resource.Layout.wordcard_layout, container, false);

            // Locate the question box TextView within the fragment's container:
            TextView questionBox = (TextView)view.FindViewById(Resource.Id.word_card_question);

            // Load the flash card with the math problem:
            questionBox.Text = question;

            // Create a handler to report the answer when the math problem is tapped:
            questionBox.Click += delegate
            {
                Toast.MakeText(Activity.ApplicationContext,
                        "Answer: " + answer, ToastLength.Short).Show();
            };
            return view;
        }
    }
}