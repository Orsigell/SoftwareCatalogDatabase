using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareCatalogDatabase
{
    public partial class CollectionForm : Form
    {
        public CollectionForm(string collectioNname, DBWorker dBWorker)
        {
            InitializeComponent();
            dataGridView1.DataSource = dBWorker.GetCollectionByName(collectioNname);
            myDBWorker = dBWorker;
        }
        DBWorker myDBWorker;
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0) && (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null))
            {
                DetalProgramForm detalProgramForm = new DetalProgramForm(myDBWorker, Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));
                detalProgramForm.Show();
            }
        }
        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns["id_software"].Visible = false;
            dataGridView1.Columns["image"].Visible = false;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0) && (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null))
            {
                pictureBox1.Image = ByteToImage((byte[])dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            }
        }
        private Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            return image;
        }
    }
}
