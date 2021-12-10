using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Adisyon_Kutuphanesi;

namespace AdisyonProje
{
    public partial class Frm_Urun : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Urun()
        {
            InitializeComponent();
        }

        public void urunyukle()
        {
            Urunler urunler = new Urunler();
            gridControl1.DataSource = urunler.AktifUrunGetir(urunler);
            cmbx_urunler.Properties.DataSource = urunler.AktifUrunGetir(urunler);
            cmbx_urunler.Properties.ValueMember = "Urun_ID";
            cmbx_urunler.Properties.DisplayMember = "Urun_Ad";
        }
        public void kategoriyukle()
        {
            Kategoriler kategoriler = new Kategoriler();
            cmbx_kategoriler.Properties.DataSource = kategoriler.KategoriGetir();
            cmbx_kategoriler.Properties.ValueMember = "Kategori_ID";
            cmbx_kategoriler.Properties.DisplayMember = "Kategori_AD";
        }
        private void Frm_Urun_Load(object sender, EventArgs e)
        {
            kategoriyukle();
            urunyukle();

            
            

        }

        private void btn_urun_kayit_Click(object sender, EventArgs e)
        {
            Urunler urun=new Urunler();
            urun.Urun_Ad = txt_urun_adi.Text;
            urun.Urun_Fiyat = Convert.ToDouble(txt_urun_fiyati.Text);
            urun.Urun_Stok = 0;
            urun.Urun_Kategori_ID = Convert.ToInt32(cmbx_kategoriler.EditValue);
            urun.Urun_Aktif = 1;
            urun.UrunKayit(urun);
            MessageBox.Show(urun.hata);
            urunyukle();
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            Urunler urunler = (Urunler)e.Row;
            urunler.guncelleme(urunler, urunler.Urun_ID);
            urunyukle();
        }

        private void btn_sil_Click(object sender, EventArgs e)
        {
            MessageBox.Show("sil  komutu geldi");

            foreach (var grd in gridView1.GetSelectedRows())
            {
                Urunler urunler = (Urunler)gridView1.GetRow(grd);

                urunler.uruniptal(urunler, urunler.Urun_ID);
               

               
            }
            urunyukle();
        }

        private void btn_urun_geri_Click(object sender, EventArgs e)
        {
            Frm_Anasayfa frm_Anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
            this.Hide();
            frm_Anasayfa.Show();
        }

        private void btn_urunal_Click(object sender, EventArgs e)
        {
            Alimlar alimlar = new Alimlar();
            alimlar.Alimlar_Adet = Convert.ToInt32(txt_alinanadet.Text);
            alimlar.Alimlar_Fiyat = Convert.ToDouble(txt_alisfiyati.Text);
            alimlar.Alinan_Urun_ID = Convert.ToInt32(cmbx_urunler.EditValue);
            alimlar.Alinan_Toplam_Masraf = alimlar.Alimlar_Fiyat * alimlar.Alimlar_Adet;
            alimlar.AlimYap(alimlar);
            MessageBox.Show(alimlar.hata);
        }
    }
}