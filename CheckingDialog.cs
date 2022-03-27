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
using RepeatEnglish.Service;

namespace RepeatEnglish
{
    public class CheckingDialog : Android.Support.V4.App.DialogFragment
    {

        public CheckingDialog() { }

        public static CheckingDialog newInstance(String question, String answer)
        {
            // Instantiate the fragment class:
            CheckingDialog fragment = new CheckingDialog();

            // Pass the question and answer to the fragment:
            Bundle args = new Bundle();
            args.PutString(Const.WORD_CARD_QUESTION, question);
            args.PutString(Const.WORD_CARD_ANSWER, answer);
            fragment.Arguments = args;

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.checking_dialog_layout, container, false);

            string question = Arguments.GetString(Const.WORD_CARD_QUESTION, "");
            string answer = Arguments.GetString(Const.WORD_CARD_ANSWER, "");
            var word_answer = view.FindViewById<TextView>(Resource.Id.word_answer);
            word_answer.Text = answer;

            view.FindViewById<Button>(Resource.Id.btn_correct_answer).Click += delegate
            {
                if (WordService.incrementCorrectCheckCounter(question))
                {
                    //Toast.MakeText(Activity.ApplicationContext, "Push corect ", ToastLength.Short).Show();
                }
                Dismiss();

            };

            view.FindViewById<Button>(Resource.Id.btn_incorrect_answer).Click += delegate
            {
                if (WordService.incrementIncorrectCheckCounter(question))
                {
                    //Toast.MakeText(Activity.ApplicationContext, "Push incorect ", ToastLength.Short).Show();
                }
                Dismiss();
            };

            return view;
        }
    }
}