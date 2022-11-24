using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareCatalogDatabase
{
    public partial class CatalogForm : Form
    {
        public CatalogForm()
        {
            InitializeComponent();
        }

        const string pathDB = @"C:\Users\Destroyer\Downloads\sqlitestudio-3.3.3\SQLiteStudio\SoftwareCatalogDatabase";
        //const string pathDB = @"SoftwareCatalogDatabase.db";
        DBWorker myBDWorker;

        private void CatalogForm_Load(object sender, EventArgs e)
        {
            myBDWorker = new DBWorker(pathDB);
            FillTags();
            dataGridView1.DataSource = myBDWorker.GetSoftwareCatalogFromDB();
        }
        private void FillTags()
        {
            TagsListView.Items.Clear();
            foreach (var item in myBDWorker.GetTagsFromDB())
            {
                TagsListView.Items.Add(item.TagName).Tag = item.TagIndex;
            }
        }
        private void tagChecked(object sender, ItemCheckedEventArgs e)
        {
            List<string> tags = new List<string>();
            foreach (ListViewItem item in ((ListView)sender).CheckedItems)
            {
                tags.Add(item.Tag.ToString());
            }
            if (tags.Count > 0)
            {
                dataGridView1.DataSource = myBDWorker.GetSoftwareCatalogFromDB(tags);
            }
            else
            {
                dataGridView1.DataSource = myBDWorker.GetSoftwareCatalogFromDB();
            }
        }
    }
}
