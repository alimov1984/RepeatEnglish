using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

namespace RepeatEnglish.Repository
{
    public static class Database
    {
        public static readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "repeatEng.db3");
        private static readonly string path = "Data Source=" + dbPath;
        /// <summary>
        /// Объявление делегата
        /// </summary>
        /// <typeparam name="T">Ссылка на класс</typeparam>
        /// <param name="reader">Ссылка на строку в таблице бд</param>
        /// <returns>Ссылка на класс</returns>
        /// 
        public delegate T DelegateReaderConstructor<T>(SqliteDataReader reader);

        /// <summary>
        /// Первоначальный размер коллекции при получении нескольких объектов
        /// </summary>
        private const int itemsCount = 20;

        /// <summary>
        /// Получение ссылки на экземпляр класса T
        /// </summary>
        /// <typeparam name="T">Название класса</typeparam>
        /// <param name="commandText">SQL запрос</param>
        /// <param name="linkToDataReaderConstructor">Ссылка на конструктор класса</param>
        /// <param name="sqlParameters">Параметры sql запроса</param>
        /// <returns>Ссылка на класс T</returns>
        public static T NGetObject<T>(string commandText, DelegateReaderConstructor<T> linkToDataReaderConstructor, params SqliteParameter[] sqlParameters)
            where T : class
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            SqliteDataReader sqlread = null;
            T result = null;
            try
            {
                using (connection = new SqliteConnection(path))
                {
                    command = new SqliteCommand(commandText, connection);
                    command.CommandType = CommandType.Text;
                    if (sqlParameters != null) command.Parameters.AddRange(sqlParameters);
                    connection.Open();
                    using (sqlread = command.ExecuteReader())
                    {
                        if (sqlread != null && sqlread.Read())
                        {
                            result = linkToDataReaderConstructor(sqlread);
                        }
                    }
                }
            }
            catch (Exception exeption)
            {
                Console.WriteLine(exeption);
                result = null;
            }
            finally
            {
                if (sqlread != null && !sqlread.IsClosed)
                    sqlread.Close();
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Получение списка ссылок на экземпляры класса T
        /// </summary>
        /// <typeparam name="T">Название класса</typeparam>
        /// <param name="commandText">SQL запрос</param>
        /// <param name="linkToDataReaderConstructor">Ссылка на конструктор класса</param>
        /// <param name="sqlParameters">Параметры sql запроса</param>
        /// <returns>Ссылка на список</returns>
        public static List<T> NGetListObjects<T>(string commandText, DelegateReaderConstructor<T> linkToDataReaderConstructor, params SqliteParameter[] sqlParameters)
            where T : class
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            SqliteDataReader sqlread = null;
            List<T> result = null;
            try
            {
                using (connection = new SqliteConnection(path))
                {
                    command = new SqliteCommand(commandText, connection);
                    command.CommandType = CommandType.Text;
                    if (sqlParameters != null) command.Parameters.AddRange(sqlParameters);
                    connection.Open();
                    using (sqlread = command.ExecuteReader())
                    {
                        if (sqlread != null)
                        {
                            result = new List<T>(itemsCount);
                            while (sqlread.Read())
                            {
                                result.Add(linkToDataReaderConstructor(sqlread));
                            }
                        }
                    }
                }
            }
            catch (Exception exeption)
            {
                result = null;
            }
            finally
            {
                if (sqlread != null && !sqlread.IsClosed)
                    sqlread.Close();
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Получение значения типа T
        /// </summary>
        /// <typeparam name="T">Название типа T</typeparam>
        /// <param name="commandText">SQL запрос</param>
        /// <returns>Экземпляр типа T</returns>
        public static T NGetValue<T>(string commandText, params SqliteParameter[] sqlParameters)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            SqliteDataReader sqlread = null;
            try
            {
                using (connection = new SqliteConnection(path))
                {
                    command = new SqliteCommand(commandText, connection);
                    command.CommandType = CommandType.Text;
                    if (sqlParameters != null) command.Parameters.AddRange(sqlParameters);
                    connection.Open();
                    object result = (T)command.ExecuteScalar();
                    return (T)result;
                }
            }
            catch (Exception exeption)
            {
                return default(T);
            }
            finally
            {
                if (sqlread != null && !sqlread.IsClosed)
                    sqlread.Close();
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }
        }

        public static int PExecQuery(string query, Hashtable ht)
        {
            SqliteConnection sqlcon = null;
            SqliteCommand sqlcom = null;
            SqliteParameter sqlparam = null;
            int res;
            try
            {
                sqlcon = new SqliteConnection(path);
                sqlcom = new SqliteCommand(query, sqlcon);
                foreach (string elem in ht.Keys)
                {
                    sqlparam = sqlcom.Parameters.AddWithValue(elem, ht[elem]);
                }
                sqlcon.Open();
                res = sqlcom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                res = -1;
            }
            finally
            {
                if (sqlcon != null && sqlcon.State != ConnectionState.Closed)
                    sqlcon.Close();
            }
            return res;
        }

        public static object PGetOne(string query, Hashtable ht)
        {
            SqliteConnection sqlcon = null;
            SqliteCommand sqlcom = null;
            SqliteParameter sqlparam = null;
            object result;
            try
            {
                sqlcon = new SqliteConnection(path);
                sqlcom = new SqliteCommand(query, sqlcon);
                foreach (string elem in ht.Keys)
                {
                    sqlparam = sqlcom.Parameters.AddWithValue(elem, ht[elem]);
                }
                sqlcon.Open();
                object obj = sqlcom.ExecuteScalar();
                if (obj != null)
                {
                    result = obj;
                }
                else
                    result = null;
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (sqlcon != null && sqlcon.State != ConnectionState.Closed)
                    sqlcon.Close();
            }
            return result;
        }

        public static Hashtable PGetRow(string query, Hashtable ht)
        {
            Hashtable result;
            SqliteConnection sqlcon = null;
            SqliteDataReader sqlread = null;
            SqliteCommand sqlcom = null;
            SqliteParameter sqlparam = null;
            try
            {
                sqlcon = new SqliteConnection(path);
                sqlcom = new SqliteCommand(query, sqlcon);
                foreach (string elem in ht.Keys)
                {
                    sqlparam = sqlcom.Parameters.AddWithValue(elem, ht[elem]);

                }
                sqlcon.Open();
                sqlread = sqlcom.ExecuteReader();
                result = new Hashtable();
                if (sqlread.HasRows)
                {
                    sqlread.Read();
                    int rcount = sqlread.FieldCount;
                    for (int i = 0; i < rcount; ++i)
                    {
                        result[sqlread.GetName(i)] = sqlread.GetValue(i);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (sqlread != null && !sqlread.IsClosed)
                    sqlread.Close();
                if (sqlcon != null && sqlcon.State != ConnectionState.Closed)
                    sqlcon.Close();
            }
            return result;
        }

        public static DataTable PGetTable(string query, Hashtable ht)
        {
            SqliteConnection sqlcon = null;
            SqliteCommand sqlcom = null;
            SqliteDataAdapter sqldata = null;
            SqliteParameter sqlparam = null;
            try
            {
                DataTable dt = new DataTable();
                sqlcon = new SqliteConnection(path);
                sqlcom = new SqliteCommand(query, sqlcon);
                foreach (string elem in ht.Keys)
                {
                    sqlparam = sqlcom.Parameters.AddWithValue(elem, ht[elem]);
                }
                sqldata = new SqliteDataAdapter(sqlcom);
                sqlcon.Open();
                sqldata.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (sqlcon != null && sqlcon.State != ConnectionState.Closed)
                    sqlcon.Close();
            }
        }
    }
}