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

namespace RepeatEnglish
{
    public static class Const
    {
        // Define Bundle keys for the word card question and answer
        public static string WORD_CARD_QUESTION = "card_question";
        public static string WORD_CARD_ANSWER = "card_answer";
        public static string SETTING_WORD_CHECKING_COUNT = "word_checking_count";
        public static readonly string CHANNEL_ID = "notifications";

    }
}