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
using Android.Support.V4.App;

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

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Repeat Your English";

            // Find the ViewPager and plug in the adapter:
            ViewPager pager = (ViewPager)FindViewById(Resource.Id.word_pager);

            // Instantiate the deck of flash cards:
            WordCardDeck wordCards = new WordCardDeck();

            TextView msgLabel = FindViewById<TextView>(Resource.Id.msgLabel);
            msgLabel.Text = "";
            if (wordCards.NumCards == 0)
            {          
                msgLabel.Text = Resources.GetString(Resource.String.checking_words_empty);
                pager.Visibility = ViewStates.Invisible;
                return;
            } else
            {
                pager.Visibility = ViewStates.Visible;
            }

            // Instantiate the adapter and pass in the deck of flash cards:
            WordCardDeckAdapter adapter = new WordCardDeckAdapter(SupportFragmentManager, wordCards);
      
            pager.Adapter = adapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_word_add)
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            if (item.ItemId == Resource.Id.menu_settings)
            {
                var intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}