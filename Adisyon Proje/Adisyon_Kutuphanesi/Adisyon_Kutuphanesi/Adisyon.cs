using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
    public class Adisyon
    {
        public int Adisyon_ID { get; set; }
        public int Adisyon_Masa_ID { get; set; }
        public DateTime Adisyon_Tarih { get; set; }
        public int Adisyon_Personel_ID { get; set; }
        public double Adisyon_Tutar { get; set; }
        public int Adisyon_Musteri_ID { get; set; }
        public byte Adisyon_aktif { get; set; }
        public bool available { get; set; }
        public bool AdisyonKontrol { get; set; }
        public string hata;
        private void AllParameters(SqlCommand cmd)
        {

            if (this.Adisyon_Masa_ID == null) cmd.Parameters.AddWithValue("@Adisyon_Masa_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_Masa_ID", this.Adisyon_Masa_ID);

            if (this.Adisyon_Tarih == null) cmd.Parameters.AddWithValue("@Adisyon_Tarih", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_Tarih", this.Adisyon_Tarih);

            if (this.Adisyon_Personel_ID == null) cmd.Parameters.AddWithValue("@Adisyon_Personel_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_Personel_ID", this.Adisyon_Personel_ID);

            if (this.Adisyon_Tutar == null) cmd.Parameters.AddWithValue("@Adisyon_Tutar", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_Tutar", this.Adisyon_Tutar);

            if (this.Adisyon_Musteri_ID == null) cmd.Parameters.AddWithValue("@Adisyon_Musteri_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_Musteri_ID", this.Adisyon_Musteri_ID);

            if (this.Adisyon_aktif == null) cmd.Parameters.AddWithValue("@Adisyon_aktif", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Adisyon_aktif", this.Adisyon_aktif);

        }

        private Adisyon FillProperty(SqlDataReader dr)
        {
            Adisyon adisyon = new Adisyon();
            adisyon.Adisyon_ID = Convert.ToInt32(dr["Adisyon_ID"]);
            if (dr["Adisyon_Masa_ID"] != DBNull.Value) adisyon.Adisyon_Masa_ID = Convert.ToInt32(dr["Adisyon_Masa_ID"]);
            if (dr["Adisyon_Tarih"] != DBNull.Value) adisyon.Adisyon_Tarih = Convert.ToDateTime(dr["Adisyon_Tarih"]);
            if (dr["Adisyon_Personel_ID"] != DBNull.Value) adisyon.Adisyon_Personel_ID = Convert.ToInt32(dr["Adisyon_Personel_ID"]);
            if (dr["Adisyon_Tutar"] != DBNull.Value) adisyon.Adisyon_Tutar = Convert.ToDouble(dr["Adisyon_Tutar"]);
            if (dr["Adisyon_Musteri_ID"] != DBNull.Value) adisyon.Adisyon_Musteri_ID = Convert.ToInt32(dr["Adisyon_Musteri_ID"]);
            if (dr["Adisyon_aktif"] != DBNull.Value) adisyon.Adisyon_aktif = Convert.ToByte(dr["Adisyon_aktif"]);

            adisyon.available = true;
            return adisyon;

        }
        public Adisyon Find(int Adisyon_ID)
        {
            VT vt = new VT();
            Adisyon header = new Adisyon();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Adisyonlar where Adisyon_ID=@Adisyon_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Adisyon_ID", Adisyon_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public Adisyon AktifAdisyonKontrol(Adisyon header,int Masa_ID)
        {
       
             VT vt = new VT();
         
                if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
                SqlCommand cmd = new SqlCommand("select * from Tbl_Adisyonlar Where Adisyon_Masa_ID=@Masa_ID AND Adisyon_Aktif='True'", vt.baglanti);
                cmd.Parameters.AddWithValue("@Masa_ID", Masa_ID);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                        dr.Read();
                    //   header= FillProperty(dr);
                    header.Adisyon_ID = Convert.ToInt32(dr["Adisyon_ID"]);
                header.Adisyon_Musteri_ID = Convert.ToInt32(dr["Adisyon_Musteri_ID"]);
                        AdisyonKontrol = true;
                    
                }
                else { AdisyonKontrol = false; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return header;

        }

        public Adisyon AdisyonKes(Adisyon Adis)
        {
          
            VT vt = new VT();
            Masa m = new Masa();
            try
            {
                if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Adisyonlar (Adisyon_Masa_ID,Adisyon_Tarih,Adisyon_Personel_ID,Adisyon_Tutar,Adisyon_Musteri_ID,Adisyon_aktif)output inserted.Adisyon_ID Values(@Adisyon_Masa_ID,@Adisyon_Tarih,@Adisyon_Personel_ID,@Adisyon_Tutar,@Adisyon_Musteri_ID,@Adisyon_aktif)", vt.baglanti);
                AllParameters(cmdekle);
                Adisyon_ID = Convert.ToInt32(cmdekle.ExecuteScalar());
                hata = "Yeni Adisyon Kesildi";
                if (Adis.Adisyon_Musteri_ID == 1)
                {
                    m.Masa_Rez = 0;
                }
                else { m.Masa_Rez = 1; }
               
                m.Masa_Durum = 1;
                m.guncelleme(m, Adisyon_Masa_ID);
            }
            catch { hata = "Adisyon Kesilemedi !"; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return Adis;
        }

        public Adisyon AdisyonGuncelle(Adisyon adis, int Adisyon_ID,double tutar)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Adisyonlar SET  
                                                            Adisyon_Tutar=@tutar
                                                            WHERE Adisyon_ID=@Adisyon_ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@Adisyon_ID", Adisyon_ID);
            cmd.Parameters.AddWithValue("@tutar", tutar);
            
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return adis;
        }

        public DataTable AdisyonGoster( int Adisyon_ID)
        {

            VT vt = new VT();
            SqlDataAdapter satisdatacmd;
            DataSet satisds;
            SqlCommandBuilder satiscmdb;

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

             satisdatacmd = new SqlDataAdapter("Select Tbl_Musteriler.Musteri_Ad,Tbl_Musteriler.Musteri_Soyad,Tbl_Urunler.Urun_Ad,Tbl_Urunler.Urun_Fiyat,Tbl_Satislar.Satis_Urun_Adet,Tbl_Adisyonlar.Adisyon_Tarih,(Tbl_Urunler.Urun_Fiyat*Tbl_Satislar.Satis_Urun_Adet)  From Tbl_Adisyonlar Join Tbl_Satislar  ON Tbl_Adisyonlar.Adisyon_ID=Tbl_Satislar.Satis_Adisyon_ID Join Tbl_Musteriler  ON Tbl_Musteriler.Musteri_ID=Tbl_Adisyonlar.Adisyon_Musteri_ID  Join Tbl_Urunler ON Tbl_Urunler.Urun_ID=Tbl_Satislar.Satis_Urun_ID where  Tbl_Satislar.Satis_Iptal='0' AND Tbl_Adisyonlar.Adisyon_ID=" + Adisyon_ID, vt.baglanti);
           
          
            satiscmdb = new SqlCommandBuilder(satisdatacmd);
            satisds = new DataSet();
            satisdatacmd.Fill(satisds, "Tbl_Adisyonlar");
           

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return satisds.Tables[0];


        }
      

        public Adisyon AdisyonKapa(Adisyon adis,int Adisyon_ID,int Adisyon_Masa_ID )
        {
            VT vt = new VT();
            Masa m = new Masa();
            Rezervasyon rezervasyon = new Rezervasyon();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            try
            {
                SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Adisyonlar SET  
                                                            Adisyon_aktif=@Adisyon_aktif
                                                            WHERE Adisyon_ID=@Adisyon_ID", vt.baglanti);

                cmd.Parameters.AddWithValue("@Adisyon_aktif", 0);
                cmd.Parameters.AddWithValue("@Adisyon_ID", Adisyon_ID);
                
                cmd.ExecuteNonQuery();
                m.Masa_Durum = 0;
                m.masarez = false;
                m.guncelleme(m, Adisyon_Masa_ID);
                AktifAdisyonKontrol(adis, Adisyon_Masa_ID);
            
                rezervasyon.RezKapa(rezervasyon, Adisyon_Masa_ID, adis.Adisyon_Musteri_ID);
            }
            catch { hata = "Adisyon Kapanamadı !"; }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return adis;
        }
    }
}
