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

namespace RepeatEnglish.Model
{
    public class Word
    {
        public long Id
        { get; set; }
        public String WordOriginal
        { get; set; }
        public String WordTranslated
        { get; set; }
        public DateTime DateCreated
        { get; set; }
        public DateTime DateUpdated
        { get; set; }
        public long UpdateCounter
        { get; set; }
        public long CorrectCheckCounter
        { get; set; }
        public long IncorrectCheckCounter
        { get; set; }

        public Word(String WordOriginal, String WordTranslated, DateTime DateCreated, DateTime DateUpdated,
            long UpdateCounter, long CorrectCheckCounter, long IncorrectCheckCounter)
        {
            this.WordOriginal = WordOriginal;
            this.WordTranslated = WordTranslated;
            this.DateCreated = DateCreated;
            this.DateUpdated = DateUpdated;
            this.UpdateCounter = UpdateCounter;
            this.CorrectCheckCounter = CorrectCheckCounter;
            this.IncorrectCheckCounter = IncorrectCheckCounter;

        }

    }
}