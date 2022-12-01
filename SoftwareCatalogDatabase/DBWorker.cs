
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
            string commandText = "Select id_software, name as Название,discription as Описание,link as Ссылка, image from software";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }
        public DataTable GetSoftwareCatalogFromDB(List<string> tags)
        {
            string commandText = $"Select id_software, name as Название,discription as Описание,link as Ссылка, image from software where ({TagsToCommand(tags)})";
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

        public List<Tag> GetTagsBySoftwareFromDB(int id_categories_group)
        {
            List<Tag> tags = new List<Tag>();
            string commandText = $"SELECT * FROM categories WHERE categories.id_categories IN (SELECT categories_id FROM  categories_group WHERE id_categories_group = {id_categories_group})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            while (SQLReader.Read())
            {
                tags.Add(new Tag(SQLReader.GetInt32(0), SQLReader.GetString(1)));
            }
            return tags;
        }

        public DataTable GetSoftwareInfoToDetalForm(int id)
        {
            string commandText = "SELECT * FROM software WHERE id_software = " + id;
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }

        internal DataTable GetSoftwareCatalogFromDBApproximately(string text)
        {
            string commandText = $"Select id_software, name as Название,discription as Описание,link as Ссылка, image from software WHERE name LIKE '%{text}%'";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }

        public List<byte[]> GetSoftwareScreensFromDB(int id_screen_group)
        {
            string commandText = $"SELECT screen FROM screens WHERE id_screens IN (SELECT screens_id FROM screen_group WHERE id_screen_group = {id_screen_group})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add((byte[])dt.Rows[i][0]);
            }
            return list;
        }
        public List<string> GetSoftwareCommentsFromDB(int id_comments_group)
        {
            string commandText = $"SELECT comment_text FROM comments WHERE id_comments IN (SELECT comments_id FROM comments_group WHERE id_comments_group = {id_comments_group})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            List<string> list = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add((string)dt.Rows[i][0]);
            }
            return list;
        }
        public DataTable GetSoftwarelicenseFromDB(int id_licenses)
        {
            string commandText = $"SELECT licenses_name as 'Название лицензии', licenses_type_name as 'Тип лицензии', licenses_price as 'Цена лицензнии', license_duration as 'Длительность лицензии' FROM licenses, licenses_type WHERE id_licenses = {id_licenses} AND licenses.id_licenses_type = licenses_type.licenses_type_id";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }
        //public void SendComment(string comment, int comment_group_id)
        //{
        //    string commandText = $"INSERT INTO comments (comment_text) VALUES ('{comment}');" +
        //        $"INSERT INTO comments_group (id_comments_group, comments_id) VALUES ({comment_group_id},last_insert_rowid())";
        //    SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
        //    Command.ExecuteNonQuery();
        //}
        public void SendComment(string comment, int comment_group_id)
        {
            string commandText = $"INSERT INTO comments (comment_text) VALUES ('{comment}');" +
                $"INSERT INTO comments_group (id_comments_group, comments_id) VALUES ({comment_group_id},last_insert_rowid())";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.ExecuteNonQuery();
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
