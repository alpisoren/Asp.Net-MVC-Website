using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
   public class Musteri
    {
        public int Musteri_ID { get; set; }
        public string Musteri_Ad { get; set; }
        public string Musteri_Soyad { get; set; }
    
        public bool available { get; set; }
        public string hata;

        public iletisim iletisimgetir = new iletisim();

        private void AllParameters(SqlCommand cmd)
        {

            if (this.Musteri_Ad == null) cmd.Parameters.AddWithValue("@Musteri_Ad", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Musteri_Ad", this.Musteri_Ad.Trim());

            if (this.Musteri_Soyad == null) cmd.Parameters.AddWithValue("@Musteri_Soyad", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Musteri_Soyad", this.Musteri_Soyad.Trim());

        }

        private Musteri FillProperty(SqlDataReader dr)
        {
            Musteri musteri = new Musteri();
            musteri.Musteri_ID = Convert.ToInt32(dr["Musteri_ID"]);
            if (dr["Musteri_Ad"] != DBNull.Value) musteri.Musteri_Ad = dr["Musteri_Ad"].ToString();
            if (dr["Musteri_Soyad"] != DBNull.Value) musteri.Musteri_Soyad = dr["Musteri_Soyad"].ToString();

            musteri.available = true;
            return musteri;

        }
        public Musteri Find(int ID)
        {
            VT vt = new VT();
            Musteri header = new Musteri();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Musteriler where Musteri_ID=@ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@ID", ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }


        public Musteri MusteriKayit(Musteri per)
        {
            Musteri musteri = new Musteri();
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            try
            {
              

                    iletisimgetir = iletisimgetir.iletisimkayit(iletisimgetir);
                    SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Musteriler (Musteri_Ad,Musteri_Soyad,Musteri_iletisim_ID) Values(@Musteri_Ad,@Musteri_Soyad,@personeliletisim)", vt.baglanti);
                    AllParameters(cmdekle);
                    cmdekle.Parameters.AddWithValue("@personeliletisim", iletisimgetir.periletisim_ID);
                    cmdekle.ExecuteNonQuery();
                    musteri.hata = "Müsteri Kaydı Başarıyla Yapıldı.";

                


            }
            catch (Exception ex) { musteri.hata = ex.ToString(); }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return musteri;
        }

        public List<Musteri> MusteriGetir()
        {
            List<Musteri> Musteriler = new List<Musteri>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Musteriler  ", vt.baglanti);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Musteriler.Add(new Musteri
                    {

                        Musteri_ID = Convert.ToInt32(dr["Musteri_ID"]),
                        Musteri_Ad = dr["Musteri_Ad"].ToString(),
                        Musteri_Soyad = dr["Musteri_Soyad"].ToString(),

                    });
                }
            }

            return Musteriler;
        }//SIKINTILI

        public Musteri MusteriGuncelle(Musteri mus, int id)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Musteriler SET  
                                                            Musteri_Ad=@Musteri_Ad,
                                                            Musteri_Soyad=@Musteri_Soyad
                                                            WHERE Musteri_ID=@ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@ID", id);
            AllParameters(cmd);
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return mus;
        }

        public DataTable listeleguncelle()
        {
            Musteri musteriler = new Musteri();
            VT vt = new VT();
            SqlDataAdapter datacmd;
            DataSet ds;
            SqlCommandBuilder cmdb;

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            datacmd = new SqlDataAdapter("select * from Tbl_Musteriler Where Musteri_ID<>1", vt.baglanti);



            cmdb = new SqlCommandBuilder(datacmd);
            ds = new DataSet();
            datacmd.Fill(ds, "Tbl_Musteriler");
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return ds.Tables[0];

        }
    }
}
