namespace SoftwareCatalogDatabase
{
    partial class CatalogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Текстовый редактор");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Разработка ПО");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Среда разработки");
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TagsListView = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(172, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(628, 450);
            this.dataGridView1.TabIndex = 4;
            // 
            // listView2
            // 
            this.TagsListView.CheckBoxes = true;
            this.TagsListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.TagsListView.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            listViewItem1.Tag = "1";
            listViewItem2.StateImageIndex = 0;
            listViewItem2.Tag = "2";
            listViewItem3.StateImageIndex = 0;
            listViewItem3.Tag = "3";
            this.TagsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.TagsListView.Location = new System.Drawing.Point(0, 0);
            this.TagsListView.Name = "listView2";
            this.TagsListView.Size = new System.Drawing.Size(172, 450);
            this.TagsListView.TabIndex = 3;
            this.TagsListView.UseCompatibleStateImageBehavior = false;
            this.TagsListView.View = System.Windows.Forms.View.List;
            this.TagsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.tagChecked);
            // 
            // CatalogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.TagsListView);
            this.Name = "CatalogForm";
            this.Text = "CatalogForm";
            this.Load += new System.EventHandler(this.CatalogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListView TagsListView;
    }
}