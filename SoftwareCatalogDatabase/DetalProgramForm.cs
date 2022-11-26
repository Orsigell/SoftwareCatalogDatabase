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
    public partial class DetalProgramForm : Form
    {
        private int id;

        public DetalProgramForm(int id)
        {
            InitializeComponent();
            MessageBox.Show(id.ToString());
            this.id = id;
        }
    }
}
