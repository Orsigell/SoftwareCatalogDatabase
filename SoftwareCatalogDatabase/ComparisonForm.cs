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
    public partial class ComparisonForm : Form
    {
        private DBWorker dBWorker;

        public ComparisonForm(DBWorker dBWorker, DataTable dt)
        {
            InitializeComponent();
            this.dBWorker = dBWorker;
            label3.Text = (string)dt.Rows[0][1];
            pictureBox1.Image = ByteToImage((byte[])dt.Rows[0][3]);
            textBox3.Text = (string)dt.Rows[0][2];
            textBox2.Text = Convert.ToString(dt.Rows[0][5]);
            pictureBox1.Image = ByteToImage((byte[])dt.Rows[0][3]);
            foreach (var item in dBWorker.GetTagsBySoftwareFromDB(Convert.ToInt32(dt.Rows[0][9])))
            {
                listView2.Items.Add(item.TagName);
            }
            dataGridView1.DataSource = dBWorker.GetSoftwareCatalogFromDB();
        }
        private Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            return image;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = dBWorker.GetSoftwareCatalogFromDBApproximately(textBox1.Text);
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
                pictureBox2.Image = ByteToImage((byte[])dataGridView1.Rows[e.RowIndex].Cells[4].Value);
                label1.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                DataTable newDt = dBWorker.GetSoftwareInfoToDetalForm(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));
                textBox4.Text = Convert.ToString(newDt.Rows[0][5]);
                textBox5.Text = Convert.ToString(newDt.Rows[0][2]);
                listView1.Items.Clear();
                foreach (var item in dBWorker.GetTagsBySoftwareFromDB(Convert.ToInt32(newDt.Rows[0][9])))
                {
                    listView1.Items.Add(item.TagName);
                }
            }
            TagComparison();
        }

        private void TagComparison()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                bool isExist = false;
                foreach (ListViewItem item2 in listView2.Items)
                {
                    if (item.Text==item2.Text)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    item.BackColor = Color.GreenYellow;
                }
            }
            foreach (ListViewItem item2 in listView2.Items)
            {
                bool isExist = false;
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Text == item2.Text)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    item2.BackColor = Color.GreenYellow;
                }
                else
                {
                    item2.BackColor = listView1.BackColor ;
                }
            }
        }
    }
}
