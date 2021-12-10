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
    public partial class Frm_Siparis : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Siparis()
        {
            InitializeComponent();
        }
        string btn,saat;
        int say = 0,saatsay=0;
        public int secili=0 ,secilisaat=0;
        public bool rezdurum=false;
        public DateTime secilitarih;
        int seciliadisyon = 0;
        public void datagetir()
        {

            Urunler urun = new Urunler();

            gridLookUpEdit1.Properties.DataSource = urun.KategoriUrunGetir(1);
            gridLookUpEdit1.Properties.ValueMember = "Urun_ID";
            gridLookUpEdit1.Properties.DisplayMember = "Urun_Ad";

            gridLookUpEdit2.Properties.DataSource = urun.KategoriUrunGetir(2);
            gridLookUpEdit2.Properties.ValueMember = "Urun_ID";
            gridLookUpEdit2.Properties.DisplayMember = "Urun_Ad";

            gridLookUpEdit3.Properties.DataSource = urun.KategoriUrunGetir(4);
            gridLookUpEdit3.Properties.ValueMember = "Urun_ID";
            gridLookUpEdit3.Properties.DisplayMember = "Urun_Ad";

            gridLookUpEdit4.Properties.DataSource = urun.KategoriUrunGetir(3);
            gridLookUpEdit4.Properties.ValueMember = "Urun_ID";
            gridLookUpEdit4.Properties.DisplayMember = "Urun_Ad";

        }
        public void satisyukle()
        {
            Satislar satis = new Satislar();
            satis.Satis_Adisyon_ID = seciliadisyon;
            satis.Satis_Iptal = 0;
            gridControl1.DataSource = satis.AktifSatislarGetir();
        }
        public void adisyonsec()
        {
            Adisyon adisyon = new Adisyon();
            adisyon.AktifAdisyonKontrol(adisyon, secili);
            seciliadisyon = adisyon.Adisyon_ID;
        }
        public void butongoster()
        {
            btn_masaekle.Enabled = true;
            btn_rezyap.Enabled = true;
            btn_siparisekleme.Enabled = true;
            btn_siparisikapa.Enabled = true;
            btn_yenisiparis.Enabled = true;
        }
        public void butongizle()
        {
            btn_masaekle.Enabled = false;
            btn_rezyap.Enabled = false;
            btn_siparisekleme.Enabled = false;
            btn_siparisikapa.Enabled = false;
            btn_yenisiparis.Enabled = false;
        }
        public void rezmusterigoster()
        {
            Musteri musteri = new Musteri();
            gridControl3.DataSource = musteri.listeleguncelle();
        }
        public void corbastokgetir()
        {
              try { 
                         Urunler urun = new Urunler();
                         urun.UrunStokGetir(urun, Convert.ToInt32(gridLookUpEdit1.EditValue));
                         txt_stok_corba.Text = urun.Urun_Stok.ToString();
                  }
            catch { }

        }
        public void anayemekstokgetir()
        {
                try
                {
                       Urunler urun = new Urunler();
                       urun.UrunStokGetir(urun, Convert.ToInt32(gridLookUpEdit2.EditValue));
                       txt_stok_anayemek.Text = urun.Urun_Stok.ToString();
                }
                catch { }
            }
        public void icecekstokgetir()
        {
                    try
                    {
                        Urunler urun = new Urunler();
                        urun.UrunStokGetir(urun, Convert.ToInt32(gridLookUpEdit3.EditValue));
                        txt_stok_icecek.Text = urun.Urun_Stok.ToString();
                    }
                    catch { }
                }
        public void tatlistokgetir()
        {
                        try
                        {
                            Urunler urun = new Urunler();
                            urun.UrunStokGetir(urun, Convert.ToInt32(gridLookUpEdit4.EditValue));
                            txt_stok_tatli.Text = urun.Urun_Stok.ToString();
                        }
                        catch { }
        }

        public void satisyap(int adisyon,int urun,int adet,byte durum)
        {
            Satislar satyemek = new Satislar();
            satyemek.Satis_Adisyon_ID = adisyon;
            satyemek.Satis_Urun_ID = urun;
            satyemek.Satis_Urun_Adet = adet;
            satyemek.Satis_Iptal = durum;

            satyemek.Satisyap();
        }

        public void masatasarim(SimpleButton YeniButon,int id)
        {
            Masa m = new Masa();
            Rezervasyon rez = new Rezervasyon();
            rez.RezMusteriGetir(id);
            ToolTip Aciklama = new ToolTip();
           
            if (m.Durumlar(id) == true)
            {
                YeniButon.Appearance.BackColor = Color.Red;
               
                if (m.Rezler(id) == true)
                {
                   
                    YeniButon.Text = "Masa " + btn + " (DOLU-REZ)\n"+ rez.Rez_Musteri_Ad + " " + rez.Rez_Musteri_Soyad;
                    
                    Aciklama.SetToolTip(YeniButon," Boş tarih olup olmadığını öğrenmek için Rezervasyon yapma butonuna basınız.. ");
                    Aciklama.ToolTipTitle = "Rezerveler :";
                }
                else
                {
                    YeniButon.Text = "Masa " + btn + " (DOLU)";
                }
            }
            else
            {
                // YeniButon.BackColor = Color.Green;
                YeniButon.Appearance.BackColor = Color.Green;

                if (m.Rezler(id) == true)
                {
                    YeniButon.Appearance.BackColor = Color.Salmon;
                  
                
                    YeniButon.Text = "Masa " + btn + " (Rezerveli) \n" + rez.Rez_Musteri_Ad + " " + rez.Rez_Musteri_Soyad;
                    //Aciklama.SetToolTip(YeniButon, " Boş tarih olup olmadığını öğrenmek için 'Rezervasyon Yap' butonuna basınız.. ");
                    //Aciklama.ToolTipTitle = "Rezerveler :";

                }
                else
                {
                    YeniButon.Text = "Masa " + btn + " (BOŞ)";
                }
            }

        }
        public void masaolusturyap(int id)
        {

            SimpleButton YeniButon = new SimpleButton();

            YeniButon.Name = id.ToString();
            btn = YeniButon.Name;
            YeniButon.Text = "Masa " + btn;
            YeniButon.Size = new Size(110, 40);
            YeniButon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;

            masatasarim(YeniButon,id);



            YeniButon.Location = new Point(60, 60);
           
            YeniButon.Click += new EventHandler(YeniButon_Click);
            flowLayoutPanel1.Controls.Add(YeniButon);
        }

    

        protected void YeniButon_Click(object sender, EventArgs e)
        {
            SimpleButton btnn = sender as SimpleButton;//sender dönen degeri gösteriyo texbox olsa texbox dönecek button oldugu için tıklanan butonun degerini alıyor
          //  MessageBox.Show(btnn.Name);
            secili = Convert.ToInt32(btnn.Name);
            label7.Text = secili.ToString();
            panelControl1.Enabled = true;
           
            label6.Text = "Masa No : " + secili;
           

            adisyonsec();
            //seçili masaya ya göre adisyon işlemi uygulancak 


        }


        public void saattasarım(HyperlinkLabelControl YeniSaat, int id ,DateTime secilitarih)
        {
            Saatler s = new Saatler();
            Rezervasyon rez = new Rezervasyon();
            s.SaatDurum(id, secili, secilitarih);
       
            if (s.saatrez!=0)
            {
                YeniSaat.Enabled = false;
                YeniSaat.BackColor = Color.Tomato;
           
            }
            else
            {
                YeniSaat.BackColor = Color.WhiteSmoke;
              
            }

        }

        public void saatolusturyap(int id, DateTime secilitarih)
        {

            HyperlinkLabelControl YeniSaat = new HyperlinkLabelControl();
            Saatler saatler = new Saatler();
            YeniSaat.Name = id.ToString();
            saat = YeniSaat.Name;
            saatler.SaatGetir(saatler,id);
            YeniSaat.Text = saatler.Saat_Saat;
            YeniSaat.Size = new Size(110, 40);
            // YeniSaat.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;

            //   masatasarim(YeniSaat, id);
            saattasarım(YeniSaat, id, secilitarih);


            YeniSaat.Location = new Point(20, 20);

            YeniSaat.Click += new EventHandler(YeniSaat_Click);
            flowLayoutPanel2.Controls.Add(YeniSaat);
        }
        protected void YeniSaat_Click(object sender, EventArgs e)
        {
            HyperlinkLabelControl stt = sender as HyperlinkLabelControl;//sender dönen degeri gösteriyo texbox olsa texbox dönecek button oldugu için tıklanan butonun degerini alıyor
                                                       //  MessageBox.Show(btnn.Name);
            secilisaat = Convert.ToInt32(stt.Name);
          
            stt.BackColor = Color.LightSeaGreen;

           


        }



        private void Frm_Siparis_Load(object sender, EventArgs e)
        {
            // TODO: Bu kod satırı 'adisyon_OtomasyonDataSet.Tbl_Urunler' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
            this.tbl_UrunlerTableAdapter.Fill(this.adisyon_OtomasyonDataSet.Tbl_Urunler);
            Masa masalar = new Masa();


            for (int i = 1; i <= masalar.Masaid(); i++)
            {
                masaolusturyap(i);
            }
            datagetir();

        }

        private void btn_masaekle_Click(object sender, EventArgs e)
        {

           
            Masa masalar = new Masa();

          
            masalar.MasaEkle((masalar.Masaid() + say).ToString());
            masaolusturyap(masalar.Masaid());
            say++;
        }

        
        private void btn_siparisekleme_Click(object sender, EventArgs e)
        {
            Masa m = new Masa();
            Adisyon adisyon = new Adisyon();
            adisyon.AktifAdisyonKontrol(adisyon, secili);
           

            if (secili != 0 && m.Durumlar(secili) == true && adisyon.AdisyonKontrol == true)
            {

                label6.Text = "Masa NO: " + secili.ToString();
          
                label8.Text = "Adisyon NO: " + seciliadisyon.ToString();
                panelControl1.Visible = true;
                satisyukle();
                seciliadisyon = adisyon.Adisyon_ID;
            
              
            }
            else { MessageBox.Show("Seçili Masaya Adisyon Açılmamıştır !"); }
        }

        private void btn_yenisiparis_Click(object sender, EventArgs e)
        {
            Masa m = new Masa();
            Adisyon adis = new Adisyon();
            Rezervasyon rez = new Rezervasyon();
            Frm_Anasayfa frm_Anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
       
            rez.RezMusteriGetir(secili);
            try
            {

                if (m.Durumlar(secili) == false)
                {
                    adis.Adisyon_Masa_ID = secili;
                    if (m.Rezler(secili) == true)
                    {
                        adis.Adisyon_Musteri_ID = rez.Rez_Musteri_ID;
                    }
                    else { adis.Adisyon_Musteri_ID = 1; }
                    adis.Adisyon_Personel_ID = frm_Anasayfa.Per_ID;
                    adis.Adisyon_Tarih = DateTime.Now;
                    adis.Adisyon_Tutar = 0;
                    adis.Adisyon_aktif = 1;
                    adis.AdisyonKes(adis);
                    this.Close();
                    Frm_Siparis siparis = new Frm_Siparis();
                    siparis.Show();
                    panelControl1.Visible = true;
                    label6.Text = secili.ToString();
                    label8.Text = adis.Adisyon_ID.ToString();
                    seciliadisyon = adis.Adisyon_ID;
                    MessageBox.Show(adis.hata);
                }
                else { MessageBox.Show("Zaten Adison Kesilmiş Masaya Yeni Adisyon Eklenemez.Lütfen Mevcut Adisyonda Düzenleme Yapınız."); }
            }
            catch { MessageBox.Show(adis.hata); }
            }

        private void btn_siparisikapa_Click(object sender, EventArgs e)
        {
            Adisyon adisyon = new Adisyon();
            Satislar satislar = new Satislar();
            adisyon.AktifAdisyonKontrol(adisyon, secili);
            if (adisyon.AdisyonKontrol == true)
            {
                string tut = satislar.Tutar(seciliadisyon).ToString();
                if (satislar.hata== "Toplama başarılı")
                {
                    gridControl2.DataSource = adisyon.AdisyonGoster(seciliadisyon);
                    txt_tutar.Text = tut+ " TL";
                    panelControl6.Visible = true;
                    butongizle();
                }
                else if (satislar.hata == "Adisyona ait herhangi bir sipariş yok") { MessageBox.Show(satislar.hata); }
               
          
            }
            else { MessageBox.Show("Seçili Masada Açık Adisyon Yok !"); }
        }
       
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridLookUpEdit1.Text != null && number_corba.Value > 0)
                {
                    if (number_corba.Value <= Convert.ToInt32(txt_stok_corba.Text))
                    {
                        satisyap(seciliadisyon, Convert.ToInt32(gridLookUpEdit1.EditValue), Convert.ToInt32(number_corba.Value), 0);
                        gridLookUpEdit1.Text = null;
                        number_corba.Value = 0;
                        satisyukle();
                    }
                    else
                    {
                        MessageBox.Show("Stokta Yeterli Ürün Bulunmamakta", "Yetersiz Stok", MessageBoxButtons.OK , MessageBoxIcon.Exclamation);
                    }
                   
                }
                if (gridLookUpEdit2.EditValue != null && number_anayemek.Value > 0)
                {
                    if (number_corba.Value <= Convert.ToInt32(txt_stok_anayemek.Text))
                    {
                        satisyap(seciliadisyon, Convert.ToInt32(gridLookUpEdit2.EditValue), Convert.ToInt32(number_anayemek.Value), 0);
                    gridLookUpEdit2.EditValue = null;
                    number_anayemek.Value = 0;
                    satisyukle();
                       
                    }
                    else
                    {
                        MessageBox.Show("Stokta Yeterli Ürün Bulunmamakta", "Yetersiz Stok", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                if (gridLookUpEdit3.EditValue != null && number_icecek.Value > 0)
                {
                        if (number_corba.Value <= Convert.ToInt32(txt_stok_icecek.Text))
                        {
                            satisyap(seciliadisyon, Convert.ToInt32(gridLookUpEdit3.EditValue), Convert.ToInt32(number_icecek.Value), 0);
                    gridLookUpEdit3.EditValue = null;
                    number_icecek.Value = 0;
                    satisyukle();
                        
                    }
                    else
                    {
                        MessageBox.Show("Stokta Yeterli Ürün Bulunmamakta", "Yetersiz Stok", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                if (gridLookUpEdit4.EditValue != null && number_tatli.Value > 0)
                {
                    if (number_corba.Value <= Convert.ToInt32(txt_stok_tatli.Text))
                    {
                    satisyap(seciliadisyon, Convert.ToInt32(gridLookUpEdit4.EditValue), Convert.ToInt32(number_tatli.Value), 0);
                    gridLookUpEdit4.EditValue = null;
                    number_tatli.Value = 0;
                    satisyukle();
                       
                    }
                    else
                    {
                        MessageBox.Show("Stokta Yeterli Ürün Bulunmamakta", "Yetersiz Stok", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                Satislar satislar = new Satislar();
                Adisyon adisyon = new Adisyon();

                adisyon.AdisyonGuncelle(adisyon, seciliadisyon, satislar.Tutar(seciliadisyon));
            }
            catch
            {
                MessageBox.Show("Yanlış Bir Giriş Yöntemi Kullandınız.."); 
            }
          
            
        }


        private void btn_gizle_Click(object sender, EventArgs e)
        {
            panelControl1.Visible = false;
        }


        private void simpleButton2_Click(object sender, EventArgs e)
        {
            panelControl6.Visible = false;
            butongoster();
        }
        
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Adisyon adisyon = new Adisyon();
            adisyon.AdisyonKapa(adisyon, seciliadisyon, secili);
            butongoster();
            this.Close();
            Frm_Siparis siparis = new Frm_Siparis();
            siparis.Show();
        }

        private void btn_rezyap_Click(object sender, EventArgs e)
        {
            Masa m = new Masa();
            
            Rezervasyon rez = new Rezervasyon();
       
           
            rez.RezMusteriGetir(secili);
            try
            {

                if (m.Durumlar(secili) == false&&secili!=0)
                {
                  
                    if (m.Rezler(secili) == false)
                   {
                        //masa boş ve rez yoksa

                        panelControl7.Visible = true;
                        butongizle();
                        rezmusterigoster();
                        rezdurum = true;

                    }
                    else { MessageBox.Show("Masa Rezerve Edilmiş Durumda Lütfen Başka Masayı Deneyiniz.."); }


                }
                else { MessageBox.Show("Zaten Adison Kesilmiş Masaya Rezervasyon Yapılamaz.."); }
            }
            catch { MessageBox.Show("hata"); }

        }

        private void btn_saatgetir_Click(object sender, EventArgs e)
        {
            Saatler saatler = new Saatler();
            secilitarih = tarih_baslangic.DateTime;
            try
            {
                flowLayoutPanel2.Controls.Clear();
                for (int i = 1; i <= saatler.Saatid(); i++)
                {

                    saatolusturyap(i, secilitarih);
                }
            }
            catch { MessageBox.Show("Hata"); }
           
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {

            Satislar satislar = (Satislar)e.Row;
          
           
            satislar.guncelleme(satislar, satislar.Satis_ID);
            Adisyon adisyon = new Adisyon();

            adisyon.AdisyonGuncelle(adisyon, seciliadisyon, satislar.Tutar(seciliadisyon));
            corbastokgetir();
            anayemekstokgetir();
            icecekstokgetir();
            tatlistokgetir();

        }
        
        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
          //  MessageBox.Show("sil  komutu geldi");

            foreach (var grd in gridView1.GetSelectedRows())
            {
                Satislar satislar = (Satislar)gridView1.GetRow(grd);

                satislar.satisiptal(satislar, satislar.Satis_ID);
                satisyukle();
                Adisyon adisyon = new Adisyon();

                adisyon.AdisyonGuncelle(adisyon, seciliadisyon, satislar.Tutar(seciliadisyon));
            }
            corbastokgetir();
            anayemekstokgetir();
            icecekstokgetir();
            tatlistokgetir();

            //  string ahmet = (string)qwe;-> içinde string oldugunu teyid ediyorsun .
        }

        private void btn_anasayfageri_Click(object sender, EventArgs e)
        {
            Frm_Anasayfa frm_Anasayfa = (Frm_Anasayfa)Application.OpenForms["Frm_Anasayfa"];
            this.Hide();
            frm_Anasayfa.Show();
        }

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            corbastokgetir();
        }

        private void gridLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            anayemekstokgetir();
        }
       
        private void gridLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            icecekstokgetir();
        }

        private void gridLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {
            tatlistokgetir();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            butongoster();
            panelControl7.Visible = false;
            rezdurum = false;
        }

        private void btn_rezmusterikayitgit_Click(object sender, EventArgs e)
        {
            Frm_Musteri_Kayit musteri_Kayit = new Frm_Musteri_Kayit();
            this.Hide();
            musteri_Kayit.Show();
        }

        private void btn_rezkayital_Click(object sender, EventArgs e)
        {
            DataRow row = gridView6.GetDataRow(gridView6.FocusedRowHandle);
            int musteriID = Convert.ToInt32(row["Musteri_ID"].ToString());
            Rezervasyon rez = new Rezervasyon();
            try
            {
                rez.Rez_Masa_ID = secili;
                rez.Rez_Musteri_ID = musteriID;//müşteri ıd datagridden seçilerek getirecek
                rez.Rez_Baslangic = secilitarih;
                rez.Rez_Saat_ID = secilisaat;
                rez.Rez_Aktif = 1;
                rez.RezYap(rez);
                MessageBox.Show(rez.hata);
                this.Close();
                Frm_Siparis siparis = new Frm_Siparis();
                siparis.Show();
            }
            catch { MessageBox.Show("Yanlış Veya Eksik Değer Girdiniz.."); }
           
        }

    
    }
}