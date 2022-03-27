using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RepeatEnglish.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RepeatEnglish
{
    [Activity(Label = "@string/app_name")]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.settings_layout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Repeat Your English";
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);

            EditText wordCountEdit = FindViewById<EditText>(Resource.Id.wordCountEdit);

            int currentWordCount = Preferences.Get(Const.SETTING_WORD_CHECKING_COUNT, 10);
            wordCountEdit.Text = currentWordCount.ToString();

            Button saveButton = FindViewById<Button>(Resource.Id.btnSave);
            if (saveButton != null)
            {
                saveButton.Click += (sender, e) =>
                {
                    string wordCountStr = wordCountEdit.Text.Trim();
                    if (wordCountStr.Length == 0)
                    {
                        Toast.MakeText(this, "Необходимо ввести значение в поле 'Количество слов при проверке(1-100)'",
                                                       ToastLength.Short).Show();
                        return;
                    }
                    int wordCount = Convert.ToInt32(wordCountStr);
                    if (wordCount <= 0 || wordCount > 100)
                    {
                        Toast.MakeText(this, "Необходимо ввести корректное значение в поле 'Количество слов при проверке(1-100)'",
                            ToastLength.Short).Show();
                        return;
                    }
                    Preferences.Set(Const.SETTING_WORD_CHECKING_COUNT, wordCount);
                    Toast.MakeText(this, "Настройки успешно сохранены", ToastLength.Short).Show();
                };
            }

            Button exportButton = FindViewById<Button>(Resource.Id.btnExport);
            if (exportButton != null)
            {
                exportButton.Click += async (sender, e) =>
                {
                    Task<String> exportedTask = WordService.exportDbInFile();
                    String exportedFileName = await exportedTask;
                    if (exportedFileName != null)
                    {
                        Toast.MakeText(this, String.Format("Данные успешно экспортированы в файл {0}"
                            , exportedFileName), ToastLength.Short).Show();
                    }
                };
            }

            var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.text" } },
                    { DevicePlatform.Android, new[] { "text/plain" } },
                });

            var options = new PickOptions
            {
                PickerTitle = "Please select a csv file",
                FileTypes = customFileType
            };

            Button importButton = FindViewById<Button>(Resource.Id.btnImport);
            if (importButton != null)
            {
                importButton.Click += async (sender, e) =>
                {
                    Task<String> importTask = PickAndShow(null);
                    String importFileName = await importTask;
                    if (importFileName != null)
                    {
                        Toast.MakeText(this, String.Format("Данные успешно импортированы в приложение из файла {0}"
                            , importFileName), ToastLength.Short).Show();
                    }
                };
            }
        }

        private async Task<String> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        Task<String> importTask = WordService.importFileInDb(result.FullPath);
                        String importFileName = await importTask;
                        return importFileName;
                    }
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
            return null;
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
            if (item.ItemId == Resource.Id.menu_word_check)
            {
                var intent = new Intent(this, typeof(CheckActivity));
                StartActivity(intent);
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}