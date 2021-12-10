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
    public partial class Frm_Personel_Kayit : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Personel_Kayit()
        {
            InitializeComponent();
        }
        Personel per = new Personel();
        iletisim ile = new iletisim();
        public void iletisimgetir()
        {
           
            int perid = Convert.ToInt32(gridLookUpEdit1.EditValue);
            gridControl1.DataSource = ile.listeleguncelle(perid);
      
            //   kombopersonelliste.Properties.Columns["Personel_Sifre"].Visible = false; //kabul etmiyor
        }
        public void personel_kombo_yukle()
        {

           
            gridLookUpEdit1.Properties.DataSource = per.PersonelGetir();
            gridLookUpEdit1.Properties.ValueMember = "Personel_ID";
            gridLookUpEdit1.Properties.DisplayMember = "Personel_Ad";

            //  kombopersonelliste.Properties.Columns["Personel_ID"].Visible = false;
            // kombopersonelliste.Properties.Columns.GetVisibleColumn(0);


        }
        public void getdata()
        {
            //datagoster.DataSource = kg.kitapgoster();
            // dataGridView1.DataSource = per.Personellistele();
            datagrid_perbilguncell.DataSource = per.listeleguncelle();
            grd_personel_sil.DataSource = per.listeleguncelle();
            grd_eskipersonel.DataSource = per.EskiPersonelGetir();
        }
  
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Frm_Anasayfa anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
            this.Hide();
            anasayfa.Show();
        }

        private void btn_personel_kayit_Click(object sender, EventArgs e)
        {

            Personel personel = new Personel();
            iletisim iletisimtanımlama = new iletisim();
           try
            {
                personel.Personel_Ad = txt_personel__ad.Text.ToString();
                personel.Personel_Soyad = txt_personel_soyad.Text.ToString();
                personel.Personel_TC = txt_personel_TC.Text.ToString();
                personel.Personel_Maas = Convert.ToDouble(txt_personel_maas.Text);
                personel.Personel_IseGiris = txt_personel_tarih.DateTime;
                personel.Personel_Pozisyon = txt_personel_pozisyon.Text.ToString();
                personel.Personel_KullaniciAdi = txt_personel_kuladi.Text.ToString();
                personel.Personel_Sifre = txt_personel_sifre.Text.ToString();
                personel.iletisimgetir.Iletisim_Sabit_Tel = txt_personel_sabittel.Text.ToString();
                personel.iletisimgetir.Iletisim_Cep_Tel = txt_personel_ceptel.Text.ToString();
                personel.iletisimgetir.Iletisim_Mail = txt_personel_mail.Text.ToString();
                personel.iletisimgetir.Iletisim_il = txt_personel_il.Text.ToString();
                personel.iletisimgetir.Iletisim_ilce = txt_personel_ilce.Text.ToString();
                personel.iletisimgetir.Iletisim_Mahalle = txt_personel_mah.Text.ToString();
                personel.iletisimgetir.Iletisim_Sokak = txt_personel_sok.Text.ToString();
                personel.iletisimgetir.Iletisim_Bina_No = Convert.ToInt32(txt_personel_binano.Text);

           personel = personel.PersonelKayit(personel);
                MessageBox.Show(personel.hata);
                getdata();
                personel_kombo_yukle();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
  
        }

        private void Frm_Personel_Kayit_Load(object sender, EventArgs e)
        {
            getdata();
            gridView3.Columns["Personel_ID"].Visible = false;

            gridView4.Columns["Personel_IseGiris"].Visible = false;
            gridView4.Columns["Personel_ID"].Visible = false;
            gridView4.Columns["Personel_Pozisyon"].Visible = false;
            gridView4.Columns["Personel_Iletisim_ID"].Visible = false;
            gridView4.Columns["Personel_KullaniciAdi"].Visible = false;
            gridView4.Columns["Personel_Sifre"].Visible = false;
            gridView4.Columns["Personel_Maas"].Visible = false;

            personel_kombo_yukle();
          


        }
        private void güncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // per.datacmd.Update(per.ds, "Tbl_Personel");

            DataRow row = gridView3.GetDataRow(gridView3.FocusedRowHandle);

            int perID = Convert.ToInt32(row["Personel_ID"].ToString());
            Personel per = new Personel();
            per = per.Find(perID);
            if(per.available)
            {
                
                per.Personel_Ad = row["Personel_Ad"].ToString();
                per.Personel_Soyad = row["Personel_Soyad"].ToString();
                per.Personel_TC = row["Personel_TC"].ToString();
                per.Personel_Maas = Convert.ToDouble(row["Personel_Maas"].ToString());
                per.Personel_Pozisyon = row["Personel_Pozisyon"].ToString();
                per.Personel_KullaniciAdi = row["Personel_KullaniciAdi"].ToString();
                per.Personel_Sifre = row["Personel_Sifre"].ToString();
                per.Personel_IseGiris = DateTime.Now;
                per.guncelleme(per, perID);

            }
            getdata();
            personel_kombo_yukle();
        }

        private void btn_per_il_getir_Click(object sender, EventArgs e)
        {
            gridControl1.Refresh();
            iletisimgetir();
         
        }

        private void seçiliOlanıSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow row = gridView2.GetDataRow(gridView2.FocusedRowHandle);
            //  if (datagoster.Rows.Count >= 0) 
            if (gridView2.DataRowCount >= 0)
            { 
                per.PersonelCikar(Convert.ToInt32(row["Personel_ID"].ToString()));     
                getdata();
            }
            else { MessageBox.Show("Çalışan Personel Yok !"); }
            getdata();
            personel_kombo_yukle();
        }

        private void guncelleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // ile.iletisimdatacmd.Update((DataTable)gridControl1.DataSource);
            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            int ilID = Convert.ToInt32(row["Iletisim_ID"].ToString());
            iletisim ileti = new iletisim();
            ileti = ileti.Find(ilID);
            if (ileti.available)
            {
                ileti.Iletisim_Sabit_Tel = row["Iletisim_Sabit_Tel"].ToString();
                ileti.Iletisim_Cep_Tel = row["Iletisim_Cep_Tel"].ToString();
                ileti.Iletisim_Mail = row["Iletisim_Mail"].ToString();
                ileti.Iletisim_il = row["Iletisim_il"].ToString();
                ileti.Iletisim_ilce = row["Iletisim_ilce"].ToString();
                ileti.Iletisim_Mahalle = row["Iletisim_Mahalle"].ToString();
                ileti.Iletisim_Sokak = row["Iletisim_Sokak"].ToString();
                ileti.Iletisim_Bina_No = Convert.ToInt32(row["Iletisim_Bina_No"].ToString());
                ileti.iletisimguncelle(ileti, ilID);
            }
            getdata();
            personel_kombo_yukle();
        }

      

    }
}