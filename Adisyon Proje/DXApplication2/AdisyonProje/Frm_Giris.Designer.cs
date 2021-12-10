namespace AdisyonProje
{
    partial class Frm_Giris
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Giris));
            this.txt_giris_kuladi = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_giris_sifre = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_giris = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txt_giris_kuladi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_giris_sifre.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_giris_kuladi
            // 
            this.txt_giris_kuladi.EditValue = "Alp";
            this.txt_giris_kuladi.Location = new System.Drawing.Point(167, 29);
            this.txt_giris_kuladi.Name = "txt_giris_kuladi";
            this.txt_giris_kuladi.Size = new System.Drawing.Size(100, 20);
            this.txt_giris_kuladi.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kullanıcı Adı :";
            // 
            // txt_giris_sifre
            // 
            this.txt_giris_sifre.EditValue = "123";
            this.txt_giris_sifre.Location = new System.Drawing.Point(167, 64);
            this.txt_giris_sifre.Name = "txt_giris_sifre";
            this.txt_giris_sifre.Size = new System.Drawing.Size(100, 20);
            this.txt_giris_sifre.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Şifre :";
            // 
            // btn_giris
            // 
            this.btn_giris.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_giris.Appearance.Options.UseBackColor = true;
            this.btn_giris.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btn_giris.AppearanceHovered.Options.UseBackColor = true;
            this.btn_giris.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btn_giris.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_giris.ImageOptions.Image")));
            this.btn_giris.Location = new System.Drawing.Point(77, 90);
            this.btn_giris.Name = "btn_giris";
            this.btn_giris.Size = new System.Drawing.Size(128, 53);
            this.btn_giris.TabIndex = 3;
            this.btn_giris.Text = "Giriş";
            this.btn_giris.Click += new System.EventHandler(this.btn_giris_Click);
            // 
            // Frm_Giris
            // 
            this.AcceptButton = this.btn_giris;
            this.Appearance.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 152);
            this.Controls.Add(this.btn_giris);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_giris_sifre);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_giris_kuladi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Giris";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_Giris";
            ((System.ComponentModel.ISupportInitialize)(this.txt_giris_kuladi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_giris_sifre.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txt_giris_kuladi;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_giris_sifre;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SimpleButton btn_giris;
    }
}