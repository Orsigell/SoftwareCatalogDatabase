﻿using System;
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
    public partial class CatalogForm : Form
    {
        public CatalogForm()
        {
            InitializeComponent();
            MainForm = this;
        }
        public static CatalogForm MainForm;
        const string pathDB = @"SoftwareCatalogDatabase";
        DBWorker myDBWorker;

        private void CatalogForm_Load(object sender, EventArgs e)
        {
            myDBWorker = new DBWorker(pathDB);
            FillTags();
            dataGridView1.DataSource = myDBWorker.GetSoftwareCatalogFromDB();
            FillCollectionComboBox();
        }
        private void FillCollectionComboBox()
        {
            foreach (var item in myDBWorker.GetCoolectionArray())
            {
                comboBox1.Items.Add(item);
            }
        }
        private void FillTags()
        {
            TagsListView.Items.Clear();
            foreach (var item in myDBWorker.GetTagsFromDB())
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
                dataGridView1.DataSource = myDBWorker.GetSoftwareCatalogFromDB(tags);
            }
            else
            {
                dataGridView1.DataSource = myDBWorker.GetSoftwareCatalogFromDB();
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

        internal void FindAnalogues(List<DBWorker.Tag> tags)
        {
            foreach (ListViewItem item in TagsListView.Items)
            {
                bool isExists = false;
                foreach (var item2 in tags)
                {
                    if (item.Text == item2.TagName)
                    {
                        isExists = true;
                    }
                }
                item.Checked = isExists;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex >= 0) && (dataGridView1.Rows[e.RowIndex].Cells[0].Value != null))
            {
                DetalProgramForm detalProgramForm = new DetalProgramForm(myDBWorker ,Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));
                detalProgramForm.Show();
            }
        }

        private void добавитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void изменитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = myDBWorker.GetSoftwareCatalogFromDBApproximately(textBox1.Text);
        }

        private void найтиПОПоПодборкамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogForm dialogForm = new DialogForm(myDBWorker);
            if(dialogForm.ShowDialog() == DialogResult.OK)
            {
                CollectionForm collectionForm = new CollectionForm(dialogForm.CollectionName ,myDBWorker);
                collectionForm.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
