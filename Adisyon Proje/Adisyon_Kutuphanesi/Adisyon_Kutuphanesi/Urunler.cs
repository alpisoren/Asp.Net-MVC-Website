using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
   public  class Urunler
    {
        public int Urun_ID { get; set; }
        public string Urun_Ad { get; set; }
        public int Urun_Kategori_ID { get; set; }
        public double Urun_Fiyat { get; set; }
        public int Urun_Stok { get; set; }
        public byte Urun_Aktif { get; set; }
        
        public bool available { get; set; }
        public string hata;
        private void AllParameters(SqlCommand cmd)
        {

            if (this.Urun_Ad == null) cmd.Parameters.AddWithValue("@Urun_Ad", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Urun_Ad", this.Urun_Ad.Trim());

            if (this.Urun_Kategori_ID == null) cmd.Parameters.AddWithValue("@Urun_Kategori_ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Urun_Kategori_ID", this.Urun_Kategori_ID);

            if (this.Urun_Fiyat == null) cmd.Parameters.AddWithValue("@Urun_Fiyat", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Urun_Fiyat", this.Urun_Fiyat);

            if (this.Urun_Stok == null) cmd.Parameters.AddWithValue("@Urun_Stok", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Urun_Stok", this.Urun_Stok);

            if (this.Urun_Aktif == null) cmd.Parameters.AddWithValue("@Urun_Aktif", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Urun_Aktif", this.Urun_Aktif);
        }

        private Urunler FillProperty(SqlDataReader dr)
        {
            Urunler urun = new Urunler();
            urun.Urun_ID = Convert.ToInt32(dr["Urun_ID"]);
            if (dr["Urun_Ad"] != DBNull.Value) urun.Urun_Ad = dr["Urun_Ad"].ToString();
            if (dr["Urun_Kategori_ID"] != DBNull.Value) urun.Urun_Kategori_ID = Convert.ToInt32(dr["Urun_Kategori_ID"]);
            if (dr["Urun_Fiyat"] != DBNull.Value) urun.Urun_Fiyat = Convert.ToDouble(dr["Urun_Fiyat"]);
            if (dr["Urun_Stok"] != DBNull.Value) urun.Urun_Stok = Convert.ToInt32(dr["Urun_Stok"]);
            if (dr["Urun_Aktif"] != DBNull.Value) urun.Urun_Aktif = Convert.ToByte(dr["Urun_Aktif"]);
            urun.available = true;
            return urun;

        }
        public Urunler Find(int Urun_ID)
        {
            VT vt = new VT();
            Urunler header = new Urunler();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Urunler where Urun_ID=@Urun_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Urun_ID", Urun_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public List<Urunler> KategoriUrunGetir(int ID)
        {
            List<Urunler> Urunlergetir = new List<Urunler>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Urunler Where Urun_Kategori_ID=@ID AND Urun_Aktif=1", vt.baglanti);
            cmd.Parameters.AddWithValue("@ID", ID);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Urunlergetir.Add(FillProperty(dr));
                   
                  

                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return Urunlergetir;
        }
        public Urunler UrunStokGetir(Urunler urun,int Urun_ID)
        {
           // List<Urunler> Urunlergetir = new List<Urunler>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Urunler Where Urun_ID=@Urun_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Urun_ID", Urun_ID);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    // Urunlergetir.Add(FillProperty(dr));
                    urun.Urun_Stok = Convert.ToInt32(dr["Urun_Stok"]);


                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return urun;
        }

        public Urunler UrunKayit(Urunler urun)
        {
  
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            try
            {

                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Urunler (Urun_Ad,Urun_Kategori_ID,Urun_Fiyat,Urun_Stok,Urun_Aktif) Values(@Urun_Ad,@Urun_Kategori_ID,@Urun_Fiyat,@Urun_Stok,@Urun_Aktif)", vt.baglanti);
                AllParameters(cmdekle);
                cmdekle.ExecuteNonQuery();
                urun.hata = "Ürün Kaydı Başarıyla Yapıldı.";

            }
            catch (Exception ex) { urun.hata = ex.ToString(); }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return urun;
        }

        public List<Urunler> AktifUrunGetir(Urunler urun)
        {
            // List<Urunler> Urunlergetir = new List<Urunler>();
            VT vt = new VT();
            List<Urunler> Urunlergetir = new List<Urunler>();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Urunler where Urun_Aktif=@Urun_Aktif ", vt.baglanti);
            cmd.Parameters.AddWithValue("@Urun_Aktif", 1);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    // Urunlergetir.Add(FillProperty(dr));
                    Urunlergetir.Add(FillProperty(dr));


                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return Urunlergetir;
        }

        public void UrunStokGuncelle(int eksilenurunadedi,int Urun_ID)
        {
            Urunler urun = new Urunler();
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            UrunStokGetir(urun, Urun_ID);
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Urunler SET  
                                                            Urun_Stok=@Urun_Stok
                                                            WHERE Urun_ID=@Urun_ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@Urun_Stok", urun.Urun_Stok-eksilenurunadedi);
            cmd.Parameters.AddWithValue("@Urun_ID", Urun_ID);

            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
          
        }

        public Urunler guncelleme(Urunler urun, int Urun_ID)
        {
            VT vt = new VT();
            Urunler urunler = new Urunler();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            try
            {
               
                SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Urunler SET  
                                                            Urun_Fiyat=@Urun_Fiyat
                                                            WHERE Urun_ID=@Urun_ID", vt.baglanti);

                cmd.Parameters.AddWithValue("@Urun_ID", Urun_ID);
                AllParameters(cmd);
                cmd.ExecuteNonQuery();

               
                hata = "Başarılı";
            }
            catch { hata = "HATA"; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return urun;
        }
        public Urunler uruniptal(Urunler urun, int Urun_ID)
        {
           
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Urunler SET  
                                                            Urun_Aktif=@Urun_Aktif
                                                            WHERE Urun_ID=@Urun_ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@Urun_ID", Urun_ID);
            cmd.Parameters.AddWithValue("@Urun_Aktif", 0);

           
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return urun;
        }
    }
}
