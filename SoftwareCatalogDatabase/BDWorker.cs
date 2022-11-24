
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareCatalogDatabase
{
    public class DBWorker
    {
        SQLiteConnection Connection;
        public DBWorker(string DBPath)
        {
            Connection = new SQLiteConnection(@"Data Source=" + DBPath);
            Connection.Open();
        }
        ~DBWorker()
        {
            try
            {
                if (Connection != null)
                {
                    Connection.Close();
                }
            }
            catch { }
        }
        public DataTable GetSoftwareCatalogFromDB()
        {
            string commandText = "Select name,discription,link,size,ram_size from software";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }
        public DataTable GetSoftwareCatalogFromDB(List<string> tags)
        {
            string commandText = $"Select name,discription,link,size,ram_size from software where ({TagsToCommand(tags)})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }
        private string TagsToCommand(List<string> tags)
        {
            string command = "";
            foreach (var item in tags)
            {
                command += $"({item} IN (SELECT categories_id FROM categories_group where categories_group.id_categories_group = software.categories_group_id))AND";
            }
            command = command.Remove(command.Length - 3, 3);
            return command;
        }

        public struct Tag
        {
            public Tag(int tagIndex, string tagName)
            {
                TagIndex = tagIndex;
                TagName = tagName;
            }
            public int TagIndex;
            public string TagName;
        }
        public List<Tag> GetTagsFromDB()
        {
            List<Tag> tags = new List<Tag>();
            string commandText = "SELECT * FROM categories";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            while (SQLReader.Read())
            {
                tags.Add(new Tag(SQLReader.GetInt32(0), SQLReader.GetString(1)));
            }
            return tags;
        }
        //public void InsertIntoDB(string name, string surname, string patronymic, double weight, bool gender, DateTime dateOfBirth, byte[] imageArr)
        //{
        //    string commandText = $"Insert INTO Person (Name, Surname, Patronymic, Weight, Gender, Image, DateOfBirth) VALUES ('{name}', '{surname}', '{patronymic}', {weight}, {gender}, @0, @d)";
        //    SQLiteParameter param = new SQLiteParameter("@0", DbType.Binary);
        //    param.Value = imageArr;
        //    SQLiteParameter param2 = new SQLiteParameter("@d", DbType.Date);
        //    param2.Value = dateOfBirth;
        //    SQLiteCommand command = new SQLiteCommand(commandText, Connection);
        //    command.Parameters.Add(param);
        //    command.Parameters.Add(param2);
        //    command.ExecuteNonQuery();
        //}
        //public void UpdateIntoDB(string name, string surname, string patronymic, double weight, bool gender, DateTime dateOfBirth, byte[] imageArr, int id)
        //{
        //    string commandText = $"Update Person SET Name = '{name}', Surname = '{surname}', Patronymic = '{patronymic}', Weight = '{weight}', Gender = '{gender}', Image = @0, DateOfBirth = @d where PersonId = '{id}'";
        //    SQLiteCommand command = new SQLiteCommand(commandText, Connection);
        //    SQLiteParameter param = new SQLiteParameter("@0", DbType.Binary);
        //    param.Value = imageArr;
        //    SQLiteParameter param2 = new SQLiteParameter("@d", DbType.Date);
        //    param2.Value = dateOfBirth;
        //    command.Parameters.Add(param);
        //    command.Parameters.Add(param2);
        //    command.ExecuteNonQuery();
        //}
        //public void DeleteFromDB(int id)
        //{
        //    string commandText = $"Delete FROM Person Where PersonId = '{id}'";
        //    SQLiteCommand command = new SQLiteCommand(commandText, Connection);
        //    command.ExecuteNonQuery();
        //}
    }
}
