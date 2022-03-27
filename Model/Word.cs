using System;


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
        public DateTime? DateShowed
        { get; set; }
        public long AddCounter
        { get; set; }
        public long CorrectCheckCounter
        { get; set; }
        public long IncorrectCheckCounter
        { get; set; }
        public long Rating
        { get; set; }

        public Word(String WordOriginal, String WordTranslated, DateTime DateCreated, DateTime DateUpdated, DateTime? DateShowed,
           long UpdateCounter, long CorrectCheckCounter, long IncorrectCheckCounter, long Rating)
        {
            this.WordOriginal = WordOriginal;
            this.WordTranslated = WordTranslated;
            this.DateCreated = DateCreated;
            this.DateUpdated = DateUpdated;
            this.DateShowed = DateShowed;
            this.AddCounter = UpdateCounter;
            this.CorrectCheckCounter = CorrectCheckCounter;
            this.IncorrectCheckCounter = IncorrectCheckCounter;
            this.Rating = Rating;
        }

    }
}