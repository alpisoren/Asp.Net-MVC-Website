using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
   public class Satislar
    {
        public int Satis_ID { get; set; }
        public int Satis_Adisyon_ID { get; set; }
        public int Satis_Urun_ID { get; set; }
        public int Satis_Urun_Adet { get; set; }
        public byte Satis_Iptal { get; set; }
        public bool available { get; set; }
        public string hata;
        public string Urun_Ad { get; set; }
        public double Urun_Fiyat { get; set; }

        private void AllParameters(SqlCommand cmd)
        {

            if (this.Satis_Adisyon_ID == null) cmd.Parameters.AddWithValue("@Satis_Adisyon_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Satis_Adisyon_ID", this.Satis_Adisyon_ID);

            if (this.Satis_Urun_ID == null) cmd.Parameters.AddWithValue("@Satis_Urun_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Satis_Urun_ID", this.Satis_Urun_ID);

            if (this.Satis_Urun_Adet == null) cmd.Parameters.AddWithValue("@Satis_Urun_Adet", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Satis_Urun_Adet", this.Satis_Urun_Adet);

            if (this.Satis_Iptal == null) cmd.Parameters.AddWithValue("@Satis_Iptal", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Satis_Iptal", this.Satis_Iptal);

        }
 
        private Satislar FillProperty(SqlDataReader dr)
        {
            Satislar satislar = new Satislar();
            satislar.Satis_ID = Convert.ToInt32(dr["Satis_ID"]);
            if (dr["Satis_Adisyon_ID"] != DBNull.Value) satislar.Satis_Adisyon_ID = Convert.ToInt32(dr["Satis_Adisyon_ID"]);
            if (dr["Satis_Urun_ID"] != DBNull.Value) satislar.Satis_Urun_ID = Convert.ToInt32(dr["Satis_Urun_ID"]);
            if (dr["Satis_Urun_Adet"] != DBNull.Value) satislar.Satis_Urun_Adet = Convert.ToInt32(dr["Satis_Urun_Adet"]);
            if (dr["Satis_Iptal"] != DBNull.Value) satislar.Satis_Iptal = Convert.ToByte(dr["Satis_Iptal"]);

            satislar.available = true;
            return satislar;

        }

        private Satislar Fullplusproperty(SqlDataReader dr)
        {
            Satislar satislar = new Satislar();
            satislar.Satis_ID = Convert.ToInt32(dr["Satis_ID"]);
            if (dr["Satis_Adisyon_ID"] != DBNull.Value) satislar.Satis_Adisyon_ID = Convert.ToInt32(dr["Satis_Adisyon_ID"]);
            if (dr["Satis_Urun_ID"] != DBNull.Value) satislar.Satis_Urun_ID = Convert.ToInt32(dr["Satis_Urun_ID"]);
            if (dr["Satis_Urun_Adet"] != DBNull.Value) satislar.Satis_Urun_Adet = Convert.ToInt32(dr["Satis_Urun_Adet"]);
            if (dr["Satis_Iptal"] != DBNull.Value) satislar.Satis_Iptal = Convert.ToByte(dr["Satis_Iptal"]);
            if (dr["Urun_Ad"] != DBNull.Value) satislar.Urun_Ad = dr["Urun_Ad"].ToString();
            if (dr["Urun_Fiyat"] != DBNull.Value) satislar.Urun_Fiyat = Convert.ToDouble(dr["Urun_Fiyat"]);


            satislar.available = true;
            return satislar;

        }

        public Satislar Find(int Satis_ID)
        {
            VT vt = new VT();
            Satislar header = new Satislar();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Satislar where Satis_ID=@Satis_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Satis_ID", Satis_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public List<Satislar> AktifSatislarGetir()//int adisyonid,byte satisiptal
        {
            List<Satislar> satislargetir = new List<Satislar>();
            VT vt = new VT();
            //SqlDataAdapter satisdatacmd;
            //DataSet satisds;
            //SqlCommandBuilder satiscmdb;

            //if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            //satisdatacmd = new SqlDataAdapter("select * from Tbl_Satislar JOIN Tbl_Urunler ON  Tbl_Satislar.Satis_Urun_ID=Tbl_Urunler.Urun_ID Where Satis_Adisyon_ID="+adisyonid+" AND Satis_Iptal="+ satisiptal, vt.baglanti);

            //satiscmdb = new SqlCommandBuilder(satisdatacmd);
            //satisds = new DataSet();
            //satisdatacmd.Fill(satisds, "Tbl_Satislar");
            //if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            //return satisds.Tables[0];

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Satislar JOIN Tbl_Urunler ON  Tbl_Satislar.Satis_Urun_ID=Tbl_Urunler.Urun_ID Where Satis_Adisyon_ID=@Satis_Adisyon_ID AND Satis_Iptal=@Satis_Iptal", vt.baglanti);
            AllParameters(cmd);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    satislargetir.Add(Fullplusproperty(dr));
                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return satislargetir;
        }

        public Satislar Satisyap()
        {
            Satislar satis = new Satislar();
            VT vt = new VT();
            Urunler urunler = new Urunler();
            try
            {
                if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Satislar (Satis_Adisyon_ID,Satis_Urun_ID,Satis_Urun_Adet,Satis_Iptal)output inserted.Satis_ID Values(@Satis_Adisyon_ID,@Satis_Urun_ID,@Satis_Urun_Adet,@Satis_Iptal)", vt.baglanti);
                AllParameters(cmdekle);
                Satis_ID = Convert.ToInt32(cmdekle.ExecuteScalar());
                hata = "Adisyona Ekleme Yapıldı";
                urunler.UrunStokGuncelle( Satis_Urun_Adet,Satis_Urun_ID);
                
            }
            catch { hata = "Adisyona Ekleme Yapılamadı !"; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return satis;
        }
        public int SatisinAdedi(int Satis_ID)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select Satis_Urun_Adet from Tbl_Satislar where Satis_ID=@Satis_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Satis_ID", Satis_ID);
            AllParameters(cmd);
          int satisadet =Convert.ToInt32( cmd.ExecuteScalar());

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return satisadet;
        }

        public Satislar guncelleme(Satislar sat, int Satis_ID)
        {
            VT vt = new VT();
            Urunler urunler = new Urunler();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            try
            { int eskiadet = SatisinAdedi(Satis_ID);
                SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Satislar SET  
                                                            Satis_Urun_Adet=@Satis_Urun_Adet
                                                            WHERE Satis_ID=@Satis_ID", vt.baglanti);

                cmd.Parameters.AddWithValue("@Satis_ID", Satis_ID);
                AllParameters(cmd);
                cmd.ExecuteNonQuery();

                urunler.UrunStokGuncelle((sat.Satis_Urun_Adet- eskiadet ), sat.Satis_Urun_ID);
                hata = "Başarılı";
            }
            catch { hata = "HATA"; }
     
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return sat;
        }

        public Satislar satisiptal(Satislar sat, int Satis_ID)
        {
            Urunler urunler = new Urunler();
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Satislar SET  
                                                            Satis_Iptal=@Satis_Iptal
                                                            WHERE Satis_ID=@Satis_ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@Satis_ID", Satis_ID);
            cmd.Parameters.AddWithValue("@Satis_Iptal", 1);

            urunler.UrunStokGuncelle(-Satis_Urun_Adet, Satis_Urun_ID);
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return sat;//bu satışın ıd sini alıp o id ye kaıytlı adet verisini bu satıştan etkilenen urunun adet verisine ekle
        }

        public Double Tutar(int adisyonid)
        {
            VT vt = new VT();
            double sonuc=0;
            try
            {
                if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
                SqlCommand cmd = new SqlCommand("select SUM(Tbl_Urunler.Urun_Fiyat*Tbl_Satislar.Satis_Urun_Adet) from Tbl_Satislar JOIN Tbl_Urunler ON  Tbl_Satislar.Satis_Urun_ID=Tbl_Urunler.Urun_ID Where Satis_Adisyon_ID=@adisyonid AND Satis_Iptal=@Satis_Iptal", vt.baglanti);
                cmd.Parameters.AddWithValue("@adisyonid", adisyonid);
                cmd.Parameters.AddWithValue("@Satis_Iptal", 0);

                sonuc = Convert.ToDouble(cmd.ExecuteScalar());
                hata = "Toplama başarılı";
            }
            catch { hata = "Adisyona ait herhangi bir sipariş yok"; }
            return sonuc;
        }
    }
}
