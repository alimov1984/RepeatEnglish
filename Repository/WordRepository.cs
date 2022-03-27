using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RepeatEnglish.Model;
using System.Collections;
using Mono.Data.Sqlite;

namespace RepeatEnglish.Repository
{
    public static class WordRepository
    {
        public static bool initDatabase()
        {
            //clearDatabase();
            if (!File.Exists(Database.dbPath))
            {
                SqliteConnection.CreateFile(Database.dbPath);

                String script = @"CREATE TABLE [word_dictionary] (
                    word_original TEXT NOT NULL,
                    word_translated TEXT NOT NULL,
	                dateCreated INTEGER NOT NULL,
                    dateUpdated INTEGER NOT NULL,
                    dateShowed INTEGER NULL,
	                add_counter INTEGER NOT NULL,
                    correct_check_counter INTEGER NOT NULL,
	                incorrect_check_counter INTEGER NOT NULL,
                    rating INTEGER NOT NULL
                );";

                Hashtable ht = new Hashtable();
                int result = Database.PExecQuery(script, ht);
                if (result == -1)
                {
                    Console.WriteLine("WordRepository.initDatabase ERROR!");
                    return false;
                }
            }
            return true;
        }

        public static void clearDatabase()
        {
            if (File.Exists(Database.dbPath))
            {
                File.Delete(Database.dbPath);
            }
        }

        public static bool clearWordTable()
        {
            String query = @"DELETE FROM [word_dictionary]";
            int result = Database.PExecQuery(query, new Hashtable());
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Word wordConstructor(SqliteDataReader dr)
        {
            long id = (long)dr[0];
            String word_original = (String)dr[1];
            String word_translated = (String)dr[2];
            DateTime dateCreated = new DateTime((long)dr[3]);
            DateTime dateUpdated = new DateTime((long)dr[4]);
            DateTime? dateShowed = null;
            if (dr[5] != DBNull.Value)
                dateShowed = new DateTime((long)dr[5]);
            long addCounter = (long)dr[6];
            long correctCheckCounter = (long)dr[7];
            long incorrectCheckCounter = (long)dr[8];
            long rating = (long)dr[9];

            Word word = new Word(word_original, word_translated, dateCreated, dateUpdated,
                dateShowed, addCounter, correctCheckCounter, incorrectCheckCounter, rating);
            word.Id = id;
            return word;
        }

        public static Word getWordById(int id)
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated, dateShowed,
                add_counter, correct_check_counter, incorrect_check_counter, rating
                FROM [word_dictionary]
                WHERE rowid = @id";

            Word word = Database.NGetObject(query, wordConstructor, new SqliteParameter[] { new SqliteParameter("@id", id) });
            return word;
        }

        public static Word getWordByWordOriginal(String wordOriginal)
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated, dateShowed,
                add_counter, correct_check_counter, incorrect_check_counter, rating
                FROM [word_dictionary]
                WHERE word_original = @word_original";

            Word word = Database.NGetObject(query, wordConstructor, new SqliteParameter[] { new SqliteParameter("@word_original", wordOriginal) });
            return word;
        }

        public static bool insertWord(Word word)
        {
            String query = @"INSERT INTO [word_dictionary] (word_original, word_translated, dateCreated, dateUpdated,
                dateShowed, add_counter, correct_check_counter, incorrect_check_counter, rating) VALUES (@word_original, @word_translated, 
                @dateCreated, @dateUpdated,@dateShowed, @add_counter, @correct_check_counter, @incorrect_check_counter, @rating);";

            Hashtable ht = new Hashtable();
            ht["@word_original"] = word.WordOriginal.Replace(",", "");
            ht["@word_translated"] = word.WordTranslated.Replace(",", "");
            ht["@dateCreated"] = word.DateCreated.Ticks;
            ht["@dateUpdated"] = word.DateUpdated.Ticks;

            if (word.DateShowed.HasValue)
                ht["@dateShowed"] = word.DateShowed.Value.Ticks;
            else
                ht["@dateShowed"] = DBNull.Value;

            ht["@add_counter"] = word.AddCounter;
            ht["@correct_check_counter"] = word.CorrectCheckCounter;
            ht["@incorrect_check_counter"] = word.IncorrectCheckCounter;
            ht["@rating"] = word.Rating;

            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool updateWord(Word word)
        {
            String query = @"UPDATE [word_dictionary] 
            SET 
                word_original = @word_original,
                word_translated = @word_translated,
                dateUpdated = @dateUpdated,
                add_counter = @add_counter,
                correct_check_counter = @correct_check_counter,
                incorrect_check_counter = @incorrect_check_counter,
                rating = @rating
            WHERE rowid = @id";

            Hashtable ht = new Hashtable();
            ht["@id"] = word.Id;
            ht["@word_original"] = word.WordOriginal.Replace(",", "");
            ht["@word_translated"] = word.WordTranslated.Replace(",", "");
            ht["@dateUpdated"] = word.DateUpdated.Ticks;
            ht["@add_counter"] = word.AddCounter;
            ht["@correct_check_counter"] = word.CorrectCheckCounter;
            ht["@incorrect_check_counter"] = word.IncorrectCheckCounter;
            ht["@rating"] = word.Rating;

            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Word> getWordsForChecking(int wordCount)
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated, dateShowed,
                add_counter, correct_check_counter, incorrect_check_counter, rating
                FROM [word_dictionary]   
                WHERE dateShowed IS NULL OR (@ticksNow - dateShowed) > @tickPerDay
                ORDER BY rating DESC
                LIMIT @wordCount;
                ";

            List<Word> wordList = Database.NGetListObjects(query, wordConstructor,
                new SqliteParameter[]
                {
                    new SqliteParameter("@wordCount", wordCount),
                    new SqliteParameter("@ticksNow", DateTime.Now.Ticks),
                    new SqliteParameter("@tickPerDay", TimeSpan.TicksPerDay),
                });
            return wordList;
        }

        public static bool incrementWordCorrectCheckCounter(Word word)
        {
            String query = @"UPDATE [word_dictionary] 
            SET 
                correct_check_counter = @correct_check_counter,
                rating = @rating
            WHERE rowid = @id";
            //    dateUpdated = strftime('%s','now')
            Hashtable ht = new Hashtable();
            ht["@id"] = word.Id;
            ht["@correct_check_counter"] = word.CorrectCheckCounter;
            ht["@rating"] = word.Rating;
            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool incrementWordIncorrectCheckCounter(Word word)
        {
            String query = @"UPDATE [word_dictionary] 
            SET 
                incorrect_check_counter = @incorrect_check_counter,
                rating = @rating
            WHERE rowid = @id";
            // dateUpdated = strftime('%s', 'now')
            Hashtable ht = new Hashtable();
            ht["@id"] = word.Id;
            ht["@incorrect_check_counter"] = word.IncorrectCheckCounter;
            ht["@rating"] = word.Rating;
            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool updateWordDateShowed(Word word)
        {
            String query = @"UPDATE [word_dictionary] 
            SET dateShowed = @dateShowed
            WHERE rowid = @id";

            Hashtable ht = new Hashtable();
            ht["@id"] = word.Id;
            ht["@dateShowed"] = DateTime.Now.Ticks;
            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Word> getWordsForExport()
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated, dateShowed,
                add_counter, correct_check_counter, incorrect_check_counter, rating
                FROM [word_dictionary]   
                ORDER BY rowid
                ";

            List<Word> wordList = Database.NGetListObjects(query, wordConstructor, null);
            return wordList;
        }

    }
}