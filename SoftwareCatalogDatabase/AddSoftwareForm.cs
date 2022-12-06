using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareCatalogDatabase
{
    public partial class AddSoftwareForm : Form
    {
        public AddSoftwareForm(DBWorker dBWorker)
        {
            InitializeComponent();
            this.dBWorker = dBWorker;
        }
        DBWorker dBWorker;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при попытке добавления изображения");
            }
        }
        List<Image> images = new List<Image>();

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = Image.FromFile(openFileDialog.FileName);
                    images.Add(pictureBox2.Image);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при попытке добавления изображения");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                images.Remove(pictureBox2.Image);
                if (images.Count > 0)
                {
                    pictureBox2.Image = images[0];
                }
                else
                {
                    pictureBox2.Image = null;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = images[images.IndexOf(pictureBox2.Image) > 0 ? images.IndexOf(pictureBox2.Image) - 1 : images.Count - 1];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = images[images.IndexOf(pictureBox2.Image) < images.Count - 1 ? images.IndexOf(pictureBox2.Image) + 1 : 0];
        }

        private void AddSoftwareForm_Load(object sender, EventArgs e)
        {
            FillTags();
            FillLicenceType();
        }
        private void FillTags()
        {
            TagsListView.Items.Clear();
            foreach (var item in dBWorker.GetTagsFromDB())
            {
                TagsListView.Items.Add(item.TagName).Tag = item.TagIndex;
            }
        }
        private void FillLicenceType()
        {
            LicenceTypeComboBox.Items.Clear();
            foreach (string item in dBWorker.GetSoftwarelicenseTypeFromDB())
            {
                LicenceTypeComboBox.Items.Add(item);
            } 
        }
        private bool TagIsExist(string tag)
        {
            foreach (ListViewItem item in TagsListView.Items)
            {
                if (item.Text == tag)
                {
                    return true;
                }
            }
            return false;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                if (!TagIsExist(textBox5.Text))
                {
                    dBWorker.AddTag(textBox5.Text);
                    FillTags();
                }
            }
            else
            {
                MessageBox.Show("Введите название категории");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (nameTextBox.Text != "")
                {
                    if (disTextBox.Text != "")
                    {
                        if (linkTextBox.Text != "")
                        {
                            if (systemRecTextBox.Text != "")
                            {
                                if (LicenNameTextBox.Text != "")
                                {
                                    if (LicenceTypeComboBox.Text != "")
                                    {
                                        if (tags.Count > 0)
                                        {
                                            dBWorker.AddSoftware(nameTextBox.Text, disTextBox.Text, linkTextBox.Text, ImageToByte(pictureBox1.Image, pictureBox1.Image.RawFormat), systemRecTextBox.Text, ImagesToBytes(images), LicenNameTextBox.Text, numericUpDown1.Value, numericUpDown2.Value, LicenceTypeComboBox.Text, tags);
                                            CatalogForm.MainForm.CatalogUpdate();
                                            Close();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Выберите хотя бы одну категорию программы");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Выберите тип лицензии");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Введите название лицензии программы");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Введите системные требования программы");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Введите ссылку на сайт программы");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите описание программы");
                    }
                }
                else
                {
                    MessageBox.Show("Введите название программы");
                }
            }
            catch
            {
                MessageBox.Show("При добавлении программы произошла ошибка");
            }
        }

        private List<byte[]> ImagesToBytes(List<Image> images)
        {
            List<byte[]> bytes = new List<byte[]>();
            foreach (var image in images)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    bytes.Add(ms.ToArray());
                }
            }
            return bytes;
        }
        private byte[] ImageToByte(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                return imageBytes;
            }
        }
        List<string> tags = new List<string>();
        private void TagsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            tags.Clear();
            foreach (ListViewItem item in ((ListView)sender).CheckedItems)
            {
                tags.Add(item.Tag.ToString());
            }
        }
    }
}
