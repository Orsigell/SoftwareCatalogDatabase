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
    public partial class ProgrammCountByTagsForm : Form
    {
        DBWorker dBWorker;
        public ProgrammCountByTagsForm(DBWorker dBWorker)
        {
            InitializeComponent();
            this.dBWorker = dBWorker;
            FillTags();
        }
        private void FillTags()
        {
            TagsComboBox.Items.Clear();
            foreach (var item in dBWorker.GetTagsFromDB())
            {
                TagsComboBox.Items.Add(item.TagName);
            }
        }

        private void TagsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = "Количество программ с заданной категорией - " + dBWorker.GetSoftwareCountByTag(TagsComboBox.Text);
        }
    }
}
