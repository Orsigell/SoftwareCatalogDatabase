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
        BDWorker myBDWorker;

        private void CatalogForm_Load(object sender, EventArgs e)
        {
            myBDWorker = new BDWorker(pathDB);
            FillTags();
            dataGridView1.DataSource = myBDWorker.GetSoftwareCatalogFromDB();
        }
        private void FillTags()
        {

        }
        private void tagChecked(object sender, ItemCheckedEventArgs e)
        {
            List<string> tags = new List<string>();
            foreach (ListViewItem item in ((ListView)sender).CheckedItems)
            {
                tags.Add((string)item.Tag);
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
