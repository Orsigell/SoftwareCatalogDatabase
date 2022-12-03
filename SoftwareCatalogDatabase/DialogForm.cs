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
    public partial class DialogForm : Form
    {
        public DialogForm(DBWorker dBWorker)
        {
            InitializeComponent();
            FillCollectionComboBox(dBWorker);
        }

        private void FillCollectionComboBox(DBWorker dBWorker)
        {
            foreach (var item in dBWorker.GetCoolectionArray())
            {
                comboBox1.Items.Add(item);
            }
        }

        public string CollectionName;
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                CollectionName = comboBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
