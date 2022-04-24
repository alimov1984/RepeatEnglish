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
using System.Threading.Tasks;
using Android.Support.V4.App;
using System.Threading;
using RepeatEnglish.Model;
using RepeatEnglish.Repository;
using System.IO;

namespace RepeatEnglish.Service
{
    [Service]
    public class FileExportService : Android.App.Service
    {
        private bool isStarted;
        private static readonly int NOTIFICATION_ID = 10001;
        private Handler handler;
        private Action runnable;
        private HandlerThread exportThread;
        public override void OnCreate()
        {
            base.OnCreate();
            exportThread = new HandlerThread("ExportThread");
            exportThread.Start();
            handler = new Handler(exportThread.Looper);
            runnable = new Action(() =>
            {

                exportDbInFile();
                //String exportedFileName = await exportedTask;
                //ShowNotificationThatServiceIsCompleted(NOTIFICATION_ID, exportedFileName);
            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (!isStarted)
            {
                this.isStarted = true;

                // Remove the notification from the status bar.
                var notificationManager = NotificationManagerCompat.From(this);
                notificationManager.Cancel(NOTIFICATION_ID);

                handler.Post(runnable);
                StopSelf();
            }

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }

        public override void OnDestroy()
        {
            this.isStarted = false;
            // Stop the handler.
            handler.RemoveCallbacks(runnable);
            exportThread.QuitSafely();
            base.OnDestroy();
        }

        private async void exportDbInFile()
        {
            Task.Delay(10000).Wait();
            if (!Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState))
            {
                return;
            }

            List<Word> wordList = WordRepository.getWordsForExport();
            if (wordList.Count == 0)
            {
                return;
            }

            String fileName = String.Format("repeatEnglish_{0}_{1}_{2}_{3}_{4}_{5}.csv"
                  , DateTime.Now.Day
                  , DateTime.Now.Month
                  , DateTime.Now.Year
                  , DateTime.Now.Hour
                  , DateTime.Now.Minute
                  , DateTime.Now.Second);
            String filePath = Path.Combine(Application.Context.GetExternalFilesDir(null).AbsolutePath,
                fileName);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                await sw.WriteLineAsync("Id,WordOriginal,WordTranslated,DateCreated,DateUpdated,DateShowed,AddCounter,CorrectCheckCounter,IncorrectCheckCounter,Rating");
                foreach (Word word in wordList)
                {
                    await sw.WriteLineAsync(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
                        , word.Id
                        , word.WordOriginal
                        , word.WordTranslated
                        , word.DateCreated.Ticks
                        , word.DateUpdated.Ticks
                        , word.DateShowed.HasValue ? word.DateShowed.Value.Ticks : 0
                        , word.AddCounter
                        , word.CorrectCheckCounter
                        , word.IncorrectCheckCounter
                        , word.Rating));
                }
            }
            ShowNotificationThatServiceIsCompleted(NOTIFICATION_ID, filePath);
        }

        private void ShowNotificationThatServiceIsCompleted(int notificationId, String filePath)
        {
            NotificationCompat.BigTextStyle textStyle = new NotificationCompat.BigTextStyle();
            String bigText = $"Данные успешно экспортированы в файл {filePath}";
            textStyle.SetBigContentTitle("Данные успешно экспортированы в файл...");
            textStyle.BigText(bigText);
            textStyle.SetSummaryText("The summary text goes here.");

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, Const.CHANNEL_ID)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                //.SetContentText("Данные успешно экспортированы в файл...")
                .SetStyle(textStyle)
                .SetDefaults((int)NotificationDefaults.Sound);


            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                notificationBuilder.SetCategory(Notification.CategoryProgress);
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                notificationBuilder.SetPriority(((int)NotificationPriority.High));
            } else
            {
                notificationBuilder.SetChannelId(Const.CHANNEL_ID);
            }

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(notificationId, notificationBuilder.Build());


            //Notification.BigTextStyle textStyle = new Notification.BigTextStyle();
            //textStyle.BigText($"Данные успешно экспортированы в файл {filePath}");

            //Notification.Builder notificationBuilder = new Notification.Builder(this)
            //    .SetSmallIcon(Resource.Mipmap.ic_launcher)
            //    .SetContentTitle(Resources.GetString(Resource.String.app_name))
            //    .SetContentText("Данные успешно экспортированы в файл...")
            //    .SetStyle(textStyle);

            //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            //notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
        }

    }
}