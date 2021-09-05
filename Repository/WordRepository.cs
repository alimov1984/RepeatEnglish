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
            if (File.Exists(Database.dbPath))
            {
                File.Delete(Database.dbPath);
            }
            if (!File.Exists(Database.dbPath))
            {
                SqliteConnection.CreateFile(Database.dbPath);

                String script = @"CREATE TABLE [word_dictionary] (
                    word_original TEXT NOT NULL,
                    word_translated TEXT NOT NULL,
	                dateCreated INTEGER NOT NULL,
                    dateUpdated INTEGER NOT NULL,
	                update_counter INTEGER NOT NULL,
                    correct_check_counter INTEGER NOT NULL,
	                incorrect_check_counter INTEGER NOT NULL
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

        private static Word wordConstructor(SqliteDataReader dr)
        {
            long id = (long)dr[0];
            String word_original = (String)dr[1];
            String word_translated = (String)dr[2];
            DateTime dateCreated = new DateTime((long)dr[3]);
            DateTime dateUpdated = new DateTime((long)dr[4]);
            long updateCounter = (long)dr[5];
            long correctCheckCounter = (long)dr[6];
            long incorrectCheckCounter = (long)dr[7];
            Word word = new Word(word_original, word_translated, dateCreated, dateUpdated,
                updateCounter, correctCheckCounter, incorrectCheckCounter);
            word.Id = id;
            return word;
        }

        public static Word getWordById(int id)
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated,
                update_counter, correct_check_counter, incorrect_check_counter
                FROM [word_dictionary]
                WHERE rowid = @id";

            Word word = Database.NGetObject(query, wordConstructor, new SqliteParameter[] { new SqliteParameter("@id", id) });
            return word;
        }

        public static Word getWordByWordOriginal(String wordOriginal)
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated,
                update_counter, correct_check_counter, incorrect_check_counter
                FROM [word_dictionary]
                WHERE word_original = @word_original";

            Word word = Database.NGetObject(query, wordConstructor, new SqliteParameter[] { new SqliteParameter("@word_original", wordOriginal) });
            return word;
        }

        public static bool insertWord(Word word)
        {
            String query = @"INSERT INTO [word_dictionary] (word_original, word_translated, dateCreated, dateUpdated,
                update_counter, correct_check_counter, incorrect_check_counter) VALUES (@word_original, @word_translated, 
                @dateCreated, @dateUpdated,@update_counter, @correct_check_counter, @incorrect_check_counter);";

            Hashtable ht = new Hashtable();
            ht["@word_original"] = word.WordOriginal;
            ht["@word_translated"] = word.WordTranslated;
            ht["@dateCreated"] = word.DateCreated.Ticks;
            ht["@dateUpdated"] = word.DateUpdated.Ticks;
            ht["@update_counter"] = word.UpdateCounter;
            ht["@correct_check_counter"] = word.CorrectCheckCounter;
            ht["@incorrect_check_counter"] = word.IncorrectCheckCounter;

            int result = Database.PExecQuery(query, ht);
            if (result > 0)
            {
                return true;
            } else
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
                update_counter = @update_counter,
                correct_check_counter = @correct_check_counter,
                incorrect_check_counter = @incorrect_check_counter
            WHERE rowid = @id";

            Hashtable ht = new Hashtable();
            ht["@id"] = word.Id;
            ht["@word_original"] = word.WordOriginal;
            ht["@word_translated"] = word.WordTranslated;
            ht["@dateUpdated"] = word.DateUpdated.Ticks;
            ht["@update_counter"] = word.UpdateCounter;
            ht["@correct_check_counter"] = word.CorrectCheckCounter;
            ht["@incorrect_check_counter"] = word.IncorrectCheckCounter;

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

        public static List<Word> getWordsForChecking()
        {
            String query = @"SELECT rowid, word_original, word_translated, dateCreated, dateUpdated,
                update_counter, correct_check_counter, incorrect_check_counter
                FROM [word_dictionary]
                ORDER BY rowid";

            List<Word> wordList = Database.NGetListObjects(query, wordConstructor, new SqliteParameter[] { });
            return wordList;
        }

    }
}