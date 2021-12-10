using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Adisyon_Kutuphanesi;

namespace AdisyonProje
{
    public partial class Frm_Anasayfa : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Anasayfa()
        {
            InitializeComponent();
        }
        public string Per_pozisyon;
        public int Per_ID;
        public string Per_Ad;

        private void btn_personel_Click(object sender, EventArgs e)
        {
            Frm_Personel_Kayit frm_Personel_Kayit = new Frm_Personel_Kayit();
            this.Hide();
            frm_Personel_Kayit.Show();

        }

        private void btn_musteri_Click(object sender, EventArgs e)
        {
            Frm_Musteri_Kayit frm_Musteri_Kayit = new Frm_Musteri_Kayit();
            this.Hide();
            frm_Musteri_Kayit.Show();
        }

        private void Frm_Anasayfa_Load(object sender, EventArgs e)
        {
            textEdit1.Text = "Hoşgeldiniz " + Per_pozisyon + " " + Per_Ad;
            if (Per_pozisyon == "CEO") { btn_personel.Enabled = true; }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            Frm_Giris giris = new Frm_Giris();
            this.Hide();
            giris.Show();

        }

        private void btn_siparis_Click(object sender, EventArgs e)
        {
            Frm_Siparis siparis = new Frm_Siparis();
            this.Hide();
            siparis.Show();
        }

        private void btn_urun_Click(object sender, EventArgs e)
        {
            Frm_Urun urun = new Frm_Urun();
            this.Hide();
            urun.Show();
        }
    }
}
