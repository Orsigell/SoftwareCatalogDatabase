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
    public partial class UpdateSoftwareForm : Form
    {
        int id;
        public UpdateSoftwareForm(DBWorker dBWorker, int id)
        {
            InitializeComponent();
            try
            {
                this.id = id;
                this.dBWorker = dBWorker;
                FillTags();
                FillLicenceType();
                DataTable dataTable = dBWorker.GetSoftwareCatalogFromDBById(id);
                nameTextBox.Text = dataTable.Rows[0][1].ToString();
                disTextBox.Text = dataTable.Rows[0][2].ToString();
                pictureBox1.Image = ByteToImage((byte[])dataTable.Rows[0][3]);
                linkTextBox.Text = dataTable.Rows[0][4].ToString();
                systemRecTextBox.Text = dataTable.Rows[0][5].ToString();
                List<byte[]> screens = dBWorker.GetSoftwareScreensFromDB(Convert.ToInt32(dataTable.Rows[0][7]));
                images = new List<Image>();
                foreach (var item in screens)
                {
                    images.Add(ByteToImage(item));
                    pictureBox2.Image = ByteToImage(item);
                }
                DataTable license = dBWorker.GetSoftwareLicensesById(Convert.ToInt32(dataTable.Rows[0][8]));
                LicenNameTextBox.Text = license.Rows[0][1].ToString();
                numericUpDown1.Value = Convert.ToInt32(license.Rows[0][4]);
                numericUpDown2.Value = Convert.ToInt32(license.Rows[0][3]);
                LicenceTypeComboBox.Text = dBWorker.GetLicensesTypeNameById(Convert.ToInt32(license.Rows[0][2]));
                var tags = dBWorker.GetTagsBySoftwareFromDB(Convert.ToInt32(dataTable.Rows[0][9]));
                foreach (ListViewItem item in TagsListView.Items)
                {
                    foreach (var item2 in tags)
                    {
                        if (item.Text == item2.TagName)
                        {
                            item.Checked = true;
                        }
                    }
                }
                tags.Clear();
                foreach (ListViewItem item in TagsListView.CheckedItems)
                {
                    this.tags.Add(item.Tag.ToString());
                }
            }
            catch
            {
                Close();
            }
        }
        private Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            return image;
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
        List<Image> images;

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
                                            dBWorker.UpdateSoftware(id,nameTextBox.Text, disTextBox.Text, linkTextBox.Text, ImageToByte(pictureBox1.Image, pictureBox1.Image.RawFormat), systemRecTextBox.Text, ImagesToBytes(images), LicenNameTextBox.Text, numericUpDown1.Value, numericUpDown2.Value, LicenceTypeComboBox.Text, tags);
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
