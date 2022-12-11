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
                selectedSoftwareId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
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
        int selectedSoftwareId = - 1;
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
            AddSoftwareForm addSoftwareForm = new AddSoftwareForm(myDBWorker);
            addSoftwareForm.Show();
        }

        private void изменитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedSoftwareId != -1)
            {
                UpdateSoftwareForm updateSoftwareForm = new UpdateSoftwareForm(myDBWorker, selectedSoftwareId);
                updateSoftwareForm.Show();
            }
            else
            {
                MessageBox.Show("Выберите программу из списка для изменения");
            }
        }

        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedSoftwareId != -1)
            {
                if (MessageBox.Show("Вы уверены что хотите безвозвратно удалить выбранную программу и все связанные с ней данные?", "Удаление программы", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    myDBWorker.DeleteSoftware(selectedSoftwareId);
                    CatalogUpdate();
                }
            }
            else
            {
                MessageBox.Show("Выберите программу из списка для изменения");
            }
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
            if (comboBox1.Text != "")
            {
                if (selectedSoftwareId != -1)
                {
                    myDBWorker.AddSoftwareToSelection(selectedSoftwareId, comboBox1.Text);
                    if (!comboBox1.Items.Contains(comboBox1.Text))
                    {
                        comboBox1.Items.Add(comboBox1.Text);
                    }
                }
            }
        }

        public void CatalogUpdate()
        {
            FillTags();
            dataGridView1.DataSource = myDBWorker.GetSoftwareCatalogFromDB();
        }

        private void поКоличествуПрограммToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgrammCountByTagsForm programmCountByTagsForm = new ProgrammCountByTagsForm(myDBWorker);
            programmCountByTagsForm.Show();
        }
    }
}
