using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
    public class Alimlar
    {
        public int Alimlar_ID { get; set; }
        public int Alimlar_Adet { get; set; }
        public double Alimlar_Fiyat { get; set; }
        public int Alinan_Urun_ID { get; set; }
        public double Alinan_Toplam_Masraf { get; set; }
        public bool available { get; set; }
        public string hata;
        private void AllParameters(SqlCommand cmd)
        {
            if (this.Alimlar_Adet == null) cmd.Parameters.AddWithValue("@Alimlar_Adet", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Alimlar_Adet", this.Alimlar_Adet);

            if (this.Alimlar_Fiyat == null) cmd.Parameters.AddWithValue("@Alimlar_Fiyat", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Alimlar_Fiyat",this.Alimlar_Fiyat);

            if (this.Alinan_Urun_ID == null) cmd.Parameters.AddWithValue("@Alinan_Urun_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Alinan_Urun_ID", this.Alinan_Urun_ID);

            if (this.Alinan_Toplam_Masraf == null) cmd.Parameters.AddWithValue("@Alinan_Toplam_Masraf", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Alinan_Toplam_Masraf", this.Alinan_Toplam_Masraf);
        }

        private Alimlar FillProperty(SqlDataReader dr)
        {
            Alimlar alimlar = new Alimlar();
            alimlar.Alimlar_ID = Convert.ToInt32(dr["Alimlar_ID"]);
            if (dr["Alimlar_Adet"] != DBNull.Value) alimlar.Alimlar_Adet = Convert.ToInt32(dr["Alimlar_Adet"]);
            if (dr["Alimlar_Fiyat"] != DBNull.Value) alimlar.Alimlar_Fiyat = Convert.ToDouble(dr["Alimlar_Fiyat"]);
            if (dr["Alinan_Urun_ID"] != DBNull.Value) alimlar.Alinan_Urun_ID = Convert.ToInt32(dr["Alinan_Urun_ID"]);
            if (dr["Alinan_Toplam_Masraf"] != DBNull.Value) alimlar.Alinan_Toplam_Masraf = Convert.ToDouble(dr["Alinan_Toplam_Masraf"]);
            alimlar.available = true;
            return alimlar;

        }
        public Alimlar Find(int Alimlar_ID)
        {
            VT vt = new VT();
            Alimlar header = new Alimlar();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Alimlar where Alimlar_ID=@Alimlar_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Alimlar_ID", Alimlar_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public Alimlar AlimYap(Alimlar alim)
        {

            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            Urunler urunler = new Urunler();
            try
            {
                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Alimlar (Alimlar_Adet,Alimlar_Fiyat,Alinan_Urun_ID,Alinan_Toplam_Masraf) Values(@Alimlar_Adet,@Alimlar_Fiyat,@Alinan_Urun_ID,@Alinan_Toplam_Masraf)", vt.baglanti);
                AllParameters(cmdekle);
                cmdekle.ExecuteNonQuery();
                alim.hata = "Alım Başarıyla Yapıldı.";
                urunler.UrunStokGuncelle(-alim.Alimlar_Adet, alim.Alinan_Urun_ID);
            }
            catch (Exception ex) { alim.hata = ex.ToString(); }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return alim;
        }
    }
}
