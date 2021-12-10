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
    public partial class Frm_Musteri_Kayit : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Musteri_Kayit()
        {
            InitializeComponent();
        }
        iletisim ile = new iletisim();
        public void iletisimgetir()
        {

            int musid = Convert.ToInt32(kombomusteriliste.EditValue);
            grd_musteriiletisim.DataSource = ile.MusteriListele(musid);
            gridView4.Columns["Iletisim_ID"].Visible = false;
            //   kombopersonelliste.Properties.Columns["Personel_Sifre"].Visible = false; //kabul etmiyor
        }
        public void musteri_kombo_yukle()
        {
            Musteri mus = new Musteri();

            kombomusteriliste.Properties.DataSource = mus.MusteriGetir();
            kombomusteriliste.Properties.ValueMember = "Musteri_ID";
            kombomusteriliste.Properties.DisplayMember = "Musteri_Ad";
            kombomusteriliste.Properties.PopulateColumns();
        }
        public void getdata()
        {
            Musteri mus = new Musteri();
            grd_musteri_guncelle.DataSource = mus.listeleguncelle();
          
           
        }

        private void btn_musteri_geri_Click(object sender, EventArgs e)
        {
            DialogResult Rezgidilsinmi = new DialogResult();

            
            try
            {
                Frm_Siparis frm_Siparis = (Frm_Siparis)Application.OpenForms["Frm_Siparis"];
                if (frm_Siparis.rezdurum ==true)
                {
                    Rezgidilsinmi = MessageBox.Show("Rezerve Alanına Gitmek İçin Tıklayın.. ?", "Uyarı", MessageBoxButtons.OK);
                    if (Rezgidilsinmi == DialogResult.OK)
                    {
                        frm_Siparis.rezmusterigoster();
                        this.Hide();
                        frm_Siparis.Show();

                    }
                }
                else if (frm_Siparis.rezdurum == false) {
                    Frm_Anasayfa anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
                    this.Hide();
                    anasayfa.Show();
                }
            }
            catch
            {
                try
                {
                    Frm_Anasayfa anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
                    this.Hide();
                    anasayfa.Show();
                }
                catch
                { MessageBox.Show("HATA");}
                }
        }

        private void btn_musteri_kayit_Click(object sender, EventArgs e)
        {

            Musteri musteri = new Musteri();
            iletisim iletisimtanımlama = new iletisim();
            try
            {
                musteri.Musteri_Ad = txt_musteri__ad.Text.ToString();
                musteri.Musteri_Soyad = txt_musteri_soyad.Text.ToString();

                musteri.iletisimgetir.Iletisim_Sabit_Tel = txt_musteri_sabittel.Text.ToString();
                musteri.iletisimgetir.Iletisim_Cep_Tel = txt_musteri_ceptel.Text.ToString();
                musteri.iletisimgetir.Iletisim_Mail = txt_musteri_mail.Text.ToString();
                musteri.iletisimgetir.Iletisim_il = txt_musteri_il.Text.ToString();
                musteri.iletisimgetir.Iletisim_ilce = txt_musteri_ilce.Text.ToString();
                musteri.iletisimgetir.Iletisim_Mahalle = txt_musteri_mah.Text.ToString();
                musteri.iletisimgetir.Iletisim_Sokak = txt_musteri_sok.Text.ToString();
                musteri.iletisimgetir.Iletisim_Bina_No = Convert.ToInt32(txt_musteri_binano.Text);

                musteri = musteri.MusteriKayit(musteri);
                MessageBox.Show(musteri.hata);
                getdata();
                musteri_kombo_yukle();
                try
                {
                    DialogResult Rezgidilsinmi = new DialogResult();

                    Frm_Siparis frm_Siparis = (Frm_Siparis)Application.OpenForms["Frm_Siparis"];
                    if (frm_Siparis.secili != 0)
                    {
                        Rezgidilsinmi = MessageBox.Show("Rezerve Alanına Gitmek İçin Tıklayın.. ?", "Uyarı", MessageBoxButtons.OK);
                        if (Rezgidilsinmi == DialogResult.OK)
                        {
                            frm_Siparis.rezmusterigoster();
                            this.Hide();
                            frm_Siparis.Show();

                        }
                    }
                }
                catch { }
               

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Frm_Musteri_Kayit_Load(object sender, EventArgs e)
        {
            getdata();
            musteri_kombo_yukle();
            gridView1.Columns["Musteri_ID"].Visible = false;

            gridView1.Columns["Musteri_iletisim_ID"].Visible = false;

        }

        private void müşteriBilgileriniGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            int musID = Convert.ToInt32(row[0].ToString());
            Musteri mus = new Musteri();
            mus = mus.Find(musID);
            if (mus.available)
            {

                mus.Musteri_Ad = row["Musteri_Ad"].ToString();
                mus.Musteri_Soyad = row["Musteri_Soyad"].ToString();

                mus.MusteriGuncelle(mus, musID);

            }
            getdata();
            musteri_kombo_yukle();
        }

        private void kombomusteriliste_EditValueChanged(object sender, EventArgs e)
        {

            kombomusteriliste.Properties.Columns[0].Visible = false;
            kombomusteriliste.Properties.Columns[3].Visible = false;

        }

        private void btn_mus_il_getir_Click(object sender, EventArgs e)
        {
            grd_musteriiletisim.Refresh();
            iletisimgetir();
        }

        private void müşteriİletişimBilgileriniGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow row = gridView4.GetDataRow(gridView4.FocusedRowHandle);
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
            musteri_kombo_yukle();
        }

       
    }
}