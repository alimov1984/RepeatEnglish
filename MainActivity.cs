using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using RepeatEnglish.Repository;
using RepeatEnglish.Service;
using Android.Views;
using Android.Content;
using Xamarin.Essentials;

namespace RepeatEnglish
{
    [Activity(Label = "@string/app_name")]
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

            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Repeat Your English";
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);


            saveButton = FindViewById<Button>(Resource.Id.btnSave);
            wordOriginalEdit = FindViewById<EditText>(Resource.Id.wordOriginalEdit);
            wordTranslatedEdit = FindViewById<EditText>(Resource.Id.wordTranslatedEdit);

            if (saveButton != null)
            {
                saveButton.Click += (sender, e) =>
                {
                    string wordOriginal = wordOriginalEdit.Text.Trim();
                    if (wordOriginal.Length == 0)
                    {
                        Toast.MakeText(this, "Необходимо заполнить поле 'Новое слово'", ToastLength.Short).Show();
                        return;
                    }
                    string wordTranslated = wordTranslatedEdit.Text.Trim();
                    if (wordTranslated.Length == 0)
                    {
                        Toast.MakeText(this, "Необходимо заполнить поле 'Перевод'", ToastLength.Short).Show();
                        return;
                    }

                    string result = WordService.insertWord(wordOriginal, wordTranslated);
                    if (!string.IsNullOrEmpty(result))
                    {
                        wordOriginalEdit.Text = "";
                        wordTranslatedEdit.Text = "";
                        Toast.MakeText(this, result, ToastLength.Short).Show();
                    }
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
            if (item.ItemId == Resource.Id.menu_word_check)
            {
                var intent = new Intent(this, typeof(CheckActivity));
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