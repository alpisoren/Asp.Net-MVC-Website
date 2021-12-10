using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
    public class Rezervasyon
    {
        public int Rez_ID { get; set; }
        public int Rez_Masa_ID { get; set; }
        public int Rez_Musteri_ID { get; set; }
        public DateTime Rez_Baslangic { get; set; } 
        public byte Rez_Aktif { get; set; }
        public int Rez_Saat_ID { get; set; }

        public bool available { get; set; }
        public string Rez_Musteri_Ad { get; set; }
        public string Rez_Musteri_Soyad { get; set; }
        private void AllParameters(SqlCommand cmd)
        {

            if (this.Rez_Masa_ID == null) cmd.Parameters.AddWithValue("@Rez_Masa_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Rez_Masa_ID", this.Rez_Masa_ID);

            if (this.Rez_Musteri_ID == null) cmd.Parameters.AddWithValue("@Rez_Musteri_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Rez_Musteri_ID", this.Rez_Musteri_ID);

            if (this.Rez_Baslangic == null) cmd.Parameters.AddWithValue("@Rez_Baslangic", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Rez_Baslangic", this.Rez_Baslangic);

            if (this.Rez_Aktif == null) cmd.Parameters.AddWithValue("@Rez_Aktif", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Rez_Aktif", this.Rez_Aktif);

            if (this.Rez_Saat_ID == null) cmd.Parameters.AddWithValue("@Rez_Saat_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Rez_Saat_ID", this.Rez_Saat_ID);

        }
        public string hata;
        private Rezervasyon FillProperty(SqlDataReader dr)
        {
            Rezervasyon rezervasyon = new Rezervasyon();
            rezervasyon.Rez_ID = Convert.ToInt32(dr["Rez_ID"]);
            if (dr["Rez_Masa_ID"] != DBNull.Value) rezervasyon.Rez_Masa_ID = Convert.ToInt32( dr["Rez_Masa_ID"]);
            if (dr["Rez_Musteri_ID"] != DBNull.Value) rezervasyon.Rez_Musteri_ID = Convert.ToInt32(dr["Rez_Musteri_ID"]);
            if (dr["Rez_Baslangic"] != DBNull.Value) rezervasyon.Rez_Baslangic = Convert.ToDateTime(dr["Rez_Baslangic"]);
            if (dr["Rez_Aktif"] != DBNull.Value) rezervasyon.Rez_Aktif = Convert.ToByte(dr["Rez_Aktif"]);
            if (dr["Rez_Saat_ID"] != DBNull.Value) rezervasyon.Rez_Saat_ID = Convert.ToInt32(dr["Rez_Saat_ID"]);

            rezervasyon.available = true;
            return rezervasyon;

        }

        public Rezervasyon Find(int Rez_ID)
        {
            VT vt = new VT();
            Rezervasyon header = new Rezervasyon();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Rezervasyon where Rez_ID=@Rez_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Rez_ID", Rez_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public Rezervasyon RezMusteriGetir(int masaid)
        {
            Rezervasyon liste = new Rezervasyon();
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Rezervasyon JOIN Tbl_Musteriler ON Tbl_Rezervasyon.Rez_Musteri_ID=Tbl_Musteriler.Musteri_ID Where Rez_Masa_ID=@ID AND Rez_Aktif='True' ", vt.baglanti); //AND Rez_Bitis<@suan
            cmd.Parameters.AddWithValue("@ID", masaid);
     
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {

                    Rez_ID = Convert.ToInt32(dr["Rez_ID"]);
                    Rez_Masa_ID = Convert.ToInt32(dr["Rez_Masa_ID"]);
                    Rez_Musteri_ID = Convert.ToInt32(dr["Rez_Musteri_ID"]);
                    Rez_Baslangic = Convert.ToDateTime(dr["Rez_Baslangic"]);
                  
                    Rez_Musteri_Ad = dr["Musteri_Ad"].ToString();
                    Rez_Musteri_Soyad = dr["Musteri_Soyad"].ToString();
        
                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return liste;
        }

        public Rezervasyon RezKapa(Rezervasyon rez, int Rez_Masa_ID, int Rez_Musteri_ID)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Rezervasyon SET  
                                                            Rez_Aktif=@tutar
                                                            WHERE Rez_Musteri_ID=@Rez_Musteri_ID AND Rez_Masa_ID=@Rez_Masa_ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@Rez_Musteri_ID", Rez_Musteri_ID);
            cmd.Parameters.AddWithValue("@Rez_Masa_ID", Rez_Masa_ID);

            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return rez;
        }

        public Rezervasyon RezYap(Rezervasyon rez)
        {
           
            VT vt = new VT();
            Masa masa=new Masa();
            try
            {
                if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Rezervasyon (Rez_Masa_ID,Rez_Musteri_ID,Rez_Baslangic,Rez_Aktif,Rez_Saat_ID)output inserted.Rez_ID Values(@Rez_Masa_ID,@Rez_Musteri_ID,@Rez_Baslangic,@Rez_Aktif,@Rez_Saat_ID)", vt.baglanti);
                AllParameters(cmdekle);
                Rez_ID = Convert.ToInt32(cmdekle.ExecuteScalar());
                masa.Masa_Durum = 0;
                masa.Masa_Rez = 1;
                masa.guncelleme(masa, Rez_Masa_ID);
                hata = "Yeni Rezervasyon yapıldı";
                
            }
            catch { hata = " Rezervasyon yapılamadı !"; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return rez;
        }
    }
}
