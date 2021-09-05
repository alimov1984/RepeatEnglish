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
using Android.Support.V7.App;
using Android.Support.V4.View;

namespace RepeatEnglish
{
    [Activity(Label = "CheckActivity")]
    public class CheckActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the content view from the "Main" layout resource:
            SetContentView(Resource.Layout.checking_layout);

            // Instantiate the deck of flash cards:
            WordCardDeck wordCards = new WordCardDeck();

            // Instantiate the adapter and pass in the deck of flash cards:
            WordCardDeckAdapter adapter = new WordCardDeckAdapter(SupportFragmentManager, wordCards);

            // Find the ViewPager and plug in the adapter:
            ViewPager pager = (ViewPager)FindViewById(Resource.Id.word_pager);
            pager.Adapter = adapter;
        }
    }
}