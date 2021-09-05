using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RepeatEnglish.Repository;
using RepeatEnglish.Service;
using Android.Views;

namespace RepeatEnglish
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected Button saveButton = null;
        protected EditText wordOriginalEdit = null;
        protected EditText wordTranslatedEdit = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            WordRepository.initDatabase();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetActionBar(toolbar);

            ActionBar.Title = "Repeat Your English";

            saveButton = FindViewById<Button>(Resource.Id.btnSave);
            wordOriginalEdit = FindViewById<EditText>(Resource.Id.wordOriginalEdit);
            wordTranslatedEdit = FindViewById<EditText>(Resource.Id.wordTranslatedEdit);

            if (saveButton != null)
            {
                saveButton.Click += (sender, e) =>
                {
                    string wordOriginal = wordOriginalEdit.Text.Trim();
                    string wordTranslated = wordTranslatedEdit.Text.Trim();
                    string result = WordService.insertWord(wordOriginal, wordTranslated);
                    Toast.MakeText(this, result, ToastLength.Short).Show();
                };
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Top ActionBar pressed: " + item.ItemId + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }
    }
}