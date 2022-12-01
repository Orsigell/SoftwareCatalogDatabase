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
    public partial class DetalProgramForm : Form
    {
        DataTable dt;
        DBWorker dBWorker;
        List<Image> imgList;

        public DetalProgramForm(DBWorker dBWorker,int id)
        {
            InitializeComponent();
            this.dBWorker = dBWorker;
            dt = dBWorker.GetSoftwareInfoToDetalForm(id);
            label3.Text = (string)dt.Rows[0][1];
            pictureBox1.Image = ByteToImage((byte[])dt.Rows[0][3]);
            textBox1.Text = (string)dt.Rows[0][2];
            textBox3.Text = Convert.ToString(dt.Rows[0][5]);
            imgList = ByteArrToImageArr(dBWorker.GetSoftwareScreensFromDB(Convert.ToInt32(dt.Rows[0][7])));
            pictureBox2.Image = imgList[0];
            dataGridView1.DataSource = dBWorker.GetSoftwarelicenseFromDB(Convert.ToInt32(dt.Rows[0][8]));
            CommentsUpdate(dBWorker);
            foreach (var item in dBWorker.GetTagsBySoftwareFromDB(Convert.ToInt32(dt.Rows[0][9])))
            {
                listBox2.Items.Add(item.TagName);
            }
        }

        private void CommentsUpdate(DBWorker dBWorker)
        {
            listBox1.Items.Clear();
            foreach (var item in dBWorker.GetSoftwareCommentsFromDB(Convert.ToInt32(dt.Rows[0][6])))
            {
                listBox1.Items.Add(item);
            }
        }

        private List<Image> ByteArrToImageArr(List<byte[]> bytes)
        {
            List<Image> images = new List<Image>();
            foreach (var item in bytes)
            {
                images.Add(ByteToImage(item));
            }
            return images;
        }

        private Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);
            return image;
        }

        int imageIndex = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            imageIndex--;
            if (imageIndex < 0)
            {
                imageIndex = imgList.Count - 1;
            }
            pictureBox2.Image =  imgList[imageIndex];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            imageIndex++;
            if (imageIndex >= imgList.Count)
            {
                imageIndex = 0;
            }
            pictureBox2.Image = imgList[imageIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CatalogForm.MainForm.FindAnalogues(dBWorker.GetTagsBySoftwareFromDB(Convert.ToInt32(dt.Rows[0][0])));
            CatalogForm.MainForm.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                dBWorker.SendComment(textBox2.Text, Convert.ToInt32(dt.Rows[0][6]));
                CommentsUpdate(dBWorker);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ComparisonForm comparisonForm = new ComparisonForm(dBWorker, dt);
            comparisonForm.Show();
        }
    }
}
