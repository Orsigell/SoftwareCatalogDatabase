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

        public List<string> GetCoolectionArray()
        {
            string commandText = $"SELECT DISTINCT collection_group_name FROM collection_group";
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

        public int GetSoftwareCountByTag(string text)
        {
            return GetNumber($"SELECT COUNT(*) FROM categories_group WHERE categories_id = (SELECT id_categories FROM categories WHERE category_name = '{text}')");
        }

        public DataTable GetSoftwareCatalogFromDBById(int id)
        {
            return ExecuteSQLCommandAndReturnDataTable($"SELECT * FROM SOFTWARE WHERE id_software == {id}");
        }

        public DataTable GetSoftwareLicensesById(int id)
        {
            return ExecuteSQLCommandAndReturnDataTable($"SELECT * FROM licenses WHERE id_licenses == {id}");
        }

        public string GetLicensesTypeNameById(int id)
        {
            return ExecuteSQLCommandAndReturnDataTable($"SELECT licenses_type_name FROM licenses_type WHERE licenses_type_id == {id}").Rows[0][0].ToString();
        }

        public DataTable GetCollectionByName(string collectioNname)
        {
            string commandText = $"Select id_software, name as Название,discription as Описание,link as Ссылка, image from software WHERE id_software IN (SELECT software_id FROM collection_group WHERE collection_group_name='{collectioNname}')";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
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
            return ExecuteSQLCommandAndReturnDataTable("Select id_software, name as Название,discription as Описание,link as Ссылка, image from software");
        }

        private DataTable ExecuteSQLCommandAndReturnDataTable(string commandText)
        {
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(SQLReader);
            return dt;
        }

        public DataTable GetSoftwareCatalogFromDB(List<string> tags)
        {
            return ExecuteSQLCommandAndReturnDataTable($"Select id_software, name as Название,discription as Описание,link as Ссылка, image from software where ({TagsToCommand(tags)})");
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

        public List<string> GetSoftwarelicenseTypeFromDB()
        {
            string commandText = $"SELECT licenses_type_name FROM licenses_type";
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

        public void AddTag(string text)
        {
            string commandText = $"INSERT INTO categories (category_name) VALUES ('{text}');";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.ExecuteNonQuery();
        }

        public void DeleteSoftware(int selectedSoftwareId)
        {
            string commandText = $"Delete FROM software Where id_software = {selectedSoftwareId}";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.ExecuteNonQuery();
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

        public void AddSoftware(string name, string disc, string link, byte[] image, string sysReq, List<byte[]> imagesList, string licenceNameText, decimal duration, decimal price, string licenceTypeText, List<string> tags)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    SQLiteCommand sqliteCommand = new SQLiteCommand($"INSERT INTO software (name,discription,image,link,system_requirements,comments_group_id,screen_group_id,licenses_id,categories_group_id) VALUES ('{name}','{disc}',@image,'{link}','{sysReq}',(SELECT MAX (comments_group_id + 1) FROM software),{AddImagesToImageGroup(imagesList, transaction)},{AddLicence(licenceNameText, duration, price, GetLicenceTypeIdByName(licenceTypeText), transaction)},{CreateCategoriesGroup(tags, transaction)})", Connection, transaction);
                    sqliteCommand.Parameters.Add(new SQLiteParameter("@image", image));
                    sqliteCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }
        private int CreateCategoriesGroup(List<string> tags, SQLiteTransaction transaction)
        {
            int categoriesGroupId = GetNumber("SELECT MAX(id_categories_group + 1) FROM categories_group");
            foreach (var item in tags)
            {
                string commandText = $"INSERT INTO categories_group (id_categories_group, categories_id) VALUES ({categoriesGroupId},{item})";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
                Command.ExecuteNonQuery();
            }
            return categoriesGroupId;
        }

        private int GetLicenceTypeIdByName(string licenceTypeText)
        {
            if (LicenceTypeIsExsists(licenceTypeText))
            {
                return GetNumber($"SELECT licenses_type_id FROM licenses_type WHERE licenses_type_name = '{licenceTypeText}'");
            }
            else
            {
                string commandText = $"INSERT INTO licenses_type (licenses_type_name) VALUES ('{licenceTypeText}')";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
                Command.ExecuteNonQuery();
                return GetNumberLastInsertRowId();
            }
        }

        private bool LicenceTypeIsExsists(string licenceTypeText)
        {
            if (GetNumber($"SELECT COUNT(licenses_type_id) FROM licenses_type WHERE licenses_type_name = '{licenceTypeText}'") != 0)
            {
                return true;
            }
            return false;
        }

        private int AddLicence(string licenceNameText, decimal duration, decimal price, int typeId, SQLiteTransaction transaction)
        {
            string commandText = $"INSERT INTO licenses (licenses_name,id_licenses_type,licenses_price,license_duration) VALUES ('{licenceNameText}',{typeId},{(int)price},{(int)duration})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
            Command.ExecuteNonQuery();
            return GetNumberLastInsertRowId();
        }

        private int AddImagesToImageGroup(List<byte[]> imagesList, SQLiteTransaction transaction)
        {
            int imageGroupId = GetNumber("SELECT MAX(id_screen_group + 1) FROM screen_group");
            foreach (var item in imagesList)
            {
                string commandText = $"INSERT INTO screen_group (id_screen_group,screens_id) VALUES ({imageGroupId},{AddScreen(item)})";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
                Command.ExecuteNonQuery();
            }
            return imageGroupId;
        }

        private int AddScreen(byte[] image)
        {
            string commandText = "INSERT INTO screens (screen) VALUES (@img)";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.Parameters.Add(new SQLiteParameter("@img", image));
            Command.ExecuteNonQuery();
            return GetNumberLastInsertRowId(); 
        }

        private int GetNumberLastInsertRowId()
        {
            string commandText = "SELECT last_insert_rowid()";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            SQLReader.Read();
            return SQLReader.GetInt32(0);
        }
        private int GetNumber(string commandText)
        {
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            SQLiteDataReader SQLReader = Command.ExecuteReader();
            SQLReader.Read();
            return SQLReader.GetInt32(0);
        }
        public void UpdateSoftware(int id,string name, string disc, string link, byte[] image, string sysReq, List<byte[]> imagesList, string licenceNameText, decimal duration, decimal price, string licenceTypeText, List<string> tags)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    SQLiteCommand sqliteCommand = new SQLiteCommand($"Update software SET name = '{name}', discription = '{disc}', image = @img, link = '{link}', system_requirements = '{sysReq}' where id_software = {id}", Connection, transaction);
                    sqliteCommand.Parameters.Add(new SQLiteParameter("@img", image));
                    sqliteCommand.ExecuteNonQuery();
                    ReplaceScreens(imagesList,transaction,GetNumber($"SELECT screen_group_id FROM software WHERE id_software = {id}"));
                    UpdateLicence(licenceNameText, duration, price, licenceTypeText, transaction, GetNumber($"SELECT licenses_id FROM software WHERE id_software = {id}"));
                    TagsUpdate(tags, transaction, GetNumber($"SELECT categories_group_id FROM software WHERE id_software = {id}"));
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private void TagsUpdate(List<string> tags, SQLiteTransaction transaction, int tagsGroupId)
        {
            DeleteTagsFromImageGroup(transaction, tagsGroupId);
            AddTagsToTagsGroup(tags, transaction, tagsGroupId);
        }

        private void DeleteTagsFromImageGroup(SQLiteTransaction transaction, int tagsGroupId)
        {
            string commandText = $"Delete FROM categories_group Where id_categories_group = {tagsGroupId}";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
            Command.ExecuteNonQuery();
        }

        private void AddTagsToTagsGroup(List<string> tags, SQLiteTransaction transaction, int categoriesGroupId)
        {
            foreach (var item in tags)
            {
                string commandText = $"INSERT INTO categories_group (id_categories_group, categories_id) VALUES ({categoriesGroupId},{item})";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
                Command.ExecuteNonQuery();
            }
        }

        private void UpdateLicence(string licenceNameText, decimal duration, decimal price, string licenceTypeText, SQLiteTransaction transaction, int licenceId)
        {
            SQLiteCommand sqliteCommand = new SQLiteCommand($"Update licenses SET licenses_name = '{licenceNameText}',id_licenses_type = {GetNumber($"SELECT licenses_type_id FROM licenses_type WHERE licenses_type_name = '{licenceTypeText}'")}, licenses_price = {price}, license_duration = {duration} where id_licenses = {licenceId}", Connection, transaction);
            sqliteCommand.ExecuteNonQuery();
        }

        private void ReplaceScreens(List<byte[]> imagesList, SQLiteTransaction transaction, int imageGroupId)
        {
            DeleteImagesFromImageGroup(transaction, imageGroupId);
            AddImagesToImageGroup(imagesList, transaction, imageGroupId);
        }

        private void DeleteImagesFromImageGroup(SQLiteTransaction transaction, int imageGroupId)
        {
            string commandText = $"Delete FROM screen_group Where id_screen_group = {imageGroupId}";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
            Command.ExecuteNonQuery();
        }

        private void AddImagesToImageGroup(List<byte[]> imagesList, SQLiteTransaction transaction, int imageGroupId)
        {
            foreach (var item in imagesList)
            {
                string commandText = $"INSERT INTO screen_group (id_screen_group,screens_id) VALUES ({imageGroupId},{AddScreen(item)})";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connection, transaction);
                Command.ExecuteNonQuery();
            }
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

        public void AddSoftwareToSelection(int selectedSoftwareId, string name)
        {
            string commandText = $"INSERT INTO collection_group (collection_group_name,software_id) VALUES ('{name}',{selectedSoftwareId})";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.ExecuteNonQuery();
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

        public void SendComment(string comment, int comment_group_id)
        {
            string commandText = $"INSERT INTO comments (comment_text) VALUES ('{comment}');" +
                $"INSERT INTO comments_group (id_comments_group, comments_id) VALUES ({comment_group_id},last_insert_rowid())";
            SQLiteCommand Command = new SQLiteCommand(commandText, Connection);
            Command.ExecuteNonQuery();
        }
    }
}
