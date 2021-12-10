namespace AdisyonProje
{
    partial class Frm_Urun
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Urun));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btn_urun_geri = new DevExpress.XtraEditors.SimpleButton();
            this.btn_urun_kayit = new DevExpress.XtraEditors.SimpleButton();
            this.txt_urun_fiyati = new DevExpress.XtraEditors.TextEdit();
            this.txt_urun_adi = new DevExpress.XtraEditors.TextEdit();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbx_kategoriler = new DevExpress.XtraEditors.LookUpEdit();
            this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btn_sil = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Urun_ID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Urun_Ad = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Urun_Fiyat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btn_urunalgeri = new DevExpress.XtraEditors.SimpleButton();
            this.btn_urunal = new DevExpress.XtraEditors.SimpleButton();
            this.txt_alinanadet = new DevExpress.XtraEditors.TextEdit();
            this.txt_alisfiyati = new DevExpress.XtraEditors.TextEdit();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbx_urunler = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_urun_fiyati.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_urun_adi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbx_kategoriler.Properties)).BeginInit();
            this.xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_sil)).BeginInit();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alinanadet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alisfiyati.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbx_urunler.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.MaxTabPageWidth = 312;
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage2;
            this.xtraTabControl1.Size = new System.Drawing.Size(535, 299);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage2,
            this.xtraTabPage3,
            this.xtraTabPage1});
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Appearance.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage2.Appearance.Header.Options.UseBackColor = true;
            this.xtraTabPage2.Appearance.HeaderActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage2.Appearance.HeaderActive.Options.UseBackColor = true;
            this.xtraTabPage2.Controls.Add(this.panelControl1);
            this.xtraTabPage2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("xtraTabPage2.ImageOptions.Image")));
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(529, 252);
            this.xtraTabPage2.Text = "Ürün Ekle";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btn_urun_geri);
            this.panelControl1.Controls.Add(this.btn_urun_kayit);
            this.panelControl1.Controls.Add(this.txt_urun_fiyati);
            this.panelControl1.Controls.Add(this.txt_urun_adi);
            this.panelControl1.Controls.Add(this.label3);
            this.panelControl1.Controls.Add(this.label2);
            this.panelControl1.Controls.Add(this.label1);
            this.panelControl1.Controls.Add(this.cmbx_kategoriler);
            this.panelControl1.Location = new System.Drawing.Point(106, 30);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(314, 192);
            this.panelControl1.TabIndex = 6;
            // 
            // btn_urun_geri
            // 
            this.btn_urun_geri.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urun_geri.Appearance.Options.UseBackColor = true;
            this.btn_urun_geri.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urun_geri.AppearanceHovered.Options.UseBackColor = true;
            this.btn_urun_geri.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btn_urun_geri.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_urun_geri.ImageOptions.Image")));
            this.btn_urun_geri.Location = new System.Drawing.Point(183, 131);
            this.btn_urun_geri.Name = "btn_urun_geri";
            this.btn_urun_geri.Size = new System.Drawing.Size(89, 43);
            this.btn_urun_geri.TabIndex = 5;
            this.btn_urun_geri.Text = "Geri";
            this.btn_urun_geri.Click += new System.EventHandler(this.btn_urun_geri_Click);
            // 
            // btn_urun_kayit
            // 
            this.btn_urun_kayit.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urun_kayit.Appearance.Options.UseBackColor = true;
            this.btn_urun_kayit.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urun_kayit.AppearanceHovered.Options.UseBackColor = true;
            this.btn_urun_kayit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btn_urun_kayit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_urun_kayit.ImageOptions.Image")));
            this.btn_urun_kayit.Location = new System.Drawing.Point(52, 131);
            this.btn_urun_kayit.Name = "btn_urun_kayit";
            this.btn_urun_kayit.Size = new System.Drawing.Size(89, 43);
            this.btn_urun_kayit.TabIndex = 4;
            this.btn_urun_kayit.Text = "Kaydet";
            this.btn_urun_kayit.Click += new System.EventHandler(this.btn_urun_kayit_Click);
            // 
            // txt_urun_fiyati
            // 
            this.txt_urun_fiyati.Location = new System.Drawing.Point(186, 85);
            this.txt_urun_fiyati.Name = "txt_urun_fiyati";
            this.txt_urun_fiyati.Size = new System.Drawing.Size(100, 20);
            this.txt_urun_fiyati.TabIndex = 3;
            // 
            // txt_urun_adi
            // 
            this.txt_urun_adi.Location = new System.Drawing.Point(186, 52);
            this.txt_urun_adi.Name = "txt_urun_adi";
            this.txt_urun_adi.Size = new System.Drawing.Size(100, 20);
            this.txt_urun_adi.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Ürünün Fiyatı";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ürünün Adı";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ürünün Kategorisi";
            // 
            // cmbx_kategoriler
            // 
            this.cmbx_kategoriler.Location = new System.Drawing.Point(186, 22);
            this.cmbx_kategoriler.Name = "cmbx_kategoriler";
            this.cmbx_kategoriler.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbx_kategoriler.Properties.NullText = "Kategori Seçiniz...";
            this.cmbx_kategoriler.Size = new System.Drawing.Size(100, 20);
            this.cmbx_kategoriler.TabIndex = 1;
            // 
            // xtraTabPage3
            // 
            this.xtraTabPage3.Appearance.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage3.Appearance.Header.Options.UseBackColor = true;
            this.xtraTabPage3.Appearance.HeaderActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage3.Appearance.HeaderActive.Options.UseBackColor = true;
            this.xtraTabPage3.Controls.Add(this.gridControl1);
            this.xtraTabPage3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("xtraTabPage3.ImageOptions.Image")));
            this.xtraTabPage3.Name = "xtraTabPage3";
            this.xtraTabPage3.Size = new System.Drawing.Size(529, 252);
            this.xtraTabPage3.Text = "Ürün Sil Ve Güncelle";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btn_sil});
            this.gridControl1.Size = new System.Drawing.Size(529, 252);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.Urun_ID,
            this.Urun_Ad,
            this.Urun_Fiyat});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gridView1_RowUpdated);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Image = ((System.Drawing.Image)(resources.GetObject("gridColumn1.AppearanceHeader.Image")));
            this.gridColumn1.AppearanceHeader.Options.UseImage = true;
            this.gridColumn1.Caption = "btn_sil";
            this.gridColumn1.ColumnEdit = this.btn_sil;
            this.gridColumn1.FieldName = "btn_sil";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // btn_sil
            // 
            this.btn_sil.AutoHeight = false;
            this.btn_sil.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btn_sil.ContextImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_sil.ContextImageOptions.Image")));
            this.btn_sil.Name = "btn_sil";
            this.btn_sil.Click += new System.EventHandler(this.btn_sil_Click);
            // 
            // Urun_ID
            // 
            this.Urun_ID.Caption = "Urun_ID";
            this.Urun_ID.FieldName = "Urun_ID";
            this.Urun_ID.Name = "Urun_ID";
            // 
            // Urun_Ad
            // 
            this.Urun_Ad.Caption = "Urun_Ad";
            this.Urun_Ad.FieldName = "Urun_Ad";
            this.Urun_Ad.Name = "Urun_Ad";
            this.Urun_Ad.Visible = true;
            this.Urun_Ad.VisibleIndex = 1;
            // 
            // Urun_Fiyat
            // 
            this.Urun_Fiyat.Caption = "Urun_Fiyat";
            this.Urun_Fiyat.FieldName = "Urun_Fiyat";
            this.Urun_Fiyat.Name = "Urun_Fiyat";
            this.Urun_Fiyat.Visible = true;
            this.Urun_Fiyat.VisibleIndex = 2;
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Appearance.Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage1.Appearance.Header.Options.UseBackColor = true;
            this.xtraTabPage1.Appearance.HeaderActive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.xtraTabPage1.Appearance.HeaderActive.Options.UseBackColor = true;
            this.xtraTabPage1.Controls.Add(this.panelControl2);
            this.xtraTabPage1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("xtraTabPage1.ImageOptions.Image")));
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(529, 252);
            this.xtraTabPage1.Text = "Ürün Al";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btn_urunalgeri);
            this.panelControl2.Controls.Add(this.btn_urunal);
            this.panelControl2.Controls.Add(this.txt_alinanadet);
            this.panelControl2.Controls.Add(this.txt_alisfiyati);
            this.panelControl2.Controls.Add(this.label4);
            this.panelControl2.Controls.Add(this.label5);
            this.panelControl2.Controls.Add(this.label6);
            this.panelControl2.Controls.Add(this.cmbx_urunler);
            this.panelControl2.Location = new System.Drawing.Point(106, 30);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(314, 192);
            this.panelControl2.TabIndex = 7;
            // 
            // btn_urunalgeri
            // 
            this.btn_urunalgeri.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urunalgeri.Appearance.Options.UseBackColor = true;
            this.btn_urunalgeri.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urunalgeri.AppearanceHovered.Options.UseBackColor = true;
            this.btn_urunalgeri.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btn_urunalgeri.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_urunalgeri.ImageOptions.Image")));
            this.btn_urunalgeri.Location = new System.Drawing.Point(183, 131);
            this.btn_urunalgeri.Name = "btn_urunalgeri";
            this.btn_urunalgeri.Size = new System.Drawing.Size(89, 43);
            this.btn_urunalgeri.TabIndex = 5;
            this.btn_urunalgeri.Text = "Geri";
            this.btn_urunalgeri.Click += new System.EventHandler(this.btn_urun_geri_Click);
            // 
            // btn_urunal
            // 
            this.btn_urunal.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urunal.Appearance.Options.UseBackColor = true;
            this.btn_urunal.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_urunal.AppearanceHovered.Options.UseBackColor = true;
            this.btn_urunal.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btn_urunal.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_urunal.ImageOptions.Image")));
            this.btn_urunal.Location = new System.Drawing.Point(52, 131);
            this.btn_urunal.Name = "btn_urunal";
            this.btn_urunal.Size = new System.Drawing.Size(89, 43);
            this.btn_urunal.TabIndex = 4;
            this.btn_urunal.Text = "Kaydet";
            this.btn_urunal.Click += new System.EventHandler(this.btn_urunal_Click);
            // 
            // txt_alinanadet
            // 
            this.txt_alinanadet.Location = new System.Drawing.Point(186, 85);
            this.txt_alinanadet.Name = "txt_alinanadet";
            this.txt_alinanadet.Size = new System.Drawing.Size(100, 20);
            this.txt_alinanadet.TabIndex = 3;
            // 
            // txt_alisfiyati
            // 
            this.txt_alisfiyati.Location = new System.Drawing.Point(186, 52);
            this.txt_alisfiyati.Name = "txt_alisfiyati";
            this.txt_alisfiyati.Size = new System.Drawing.Size(100, 20);
            this.txt_alisfiyati.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Adet";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Ürünün Alış Fiyatı";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Alınacak Ürün";
            // 
            // cmbx_urunler
            // 
            this.cmbx_urunler.Location = new System.Drawing.Point(186, 22);
            this.cmbx_urunler.Name = "cmbx_urunler";
            this.cmbx_urunler.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbx_urunler.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Urun_ID", "Urun_ID"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Urun_Ad", "Urun_Ad")});
            this.cmbx_urunler.Properties.NullText = "Ürün Seçiniz..";
            this.cmbx_urunler.Size = new System.Drawing.Size(100, 20);
            this.cmbx_urunler.TabIndex = 1;
            // 
            // Frm_Urun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 299);
            this.Controls.Add(this.xtraTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Urun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_Rez";
            this.Load += new System.EventHandler(this.Frm_Urun_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_urun_fiyati.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_urun_adi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbx_kategoriler.Properties)).EndInit();
            this.xtraTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_sil)).EndInit();
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alinanadet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_alisfiyati.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbx_urunler.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.LookUpEdit cmbx_kategoriler;
        private DevExpress.XtraEditors.SimpleButton btn_urun_geri;
        private DevExpress.XtraEditors.SimpleButton btn_urun_kayit;
        private DevExpress.XtraEditors.TextEdit txt_urun_fiyati;
        private DevExpress.XtraEditors.TextEdit txt_urun_adi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btn_sil;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn Urun_Ad;
        private DevExpress.XtraGrid.Columns.GridColumn Urun_Fiyat;
        private DevExpress.XtraGrid.Columns.GridColumn Urun_ID;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btn_urunalgeri;
        private DevExpress.XtraEditors.SimpleButton btn_urunal;
        private DevExpress.XtraEditors.TextEdit txt_alinanadet;
        private DevExpress.XtraEditors.TextEdit txt_alisfiyati;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private DevExpress.XtraEditors.LookUpEdit cmbx_urunler;
    }
}