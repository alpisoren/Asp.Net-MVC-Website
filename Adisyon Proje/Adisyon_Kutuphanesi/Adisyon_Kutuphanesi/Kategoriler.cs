using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Adisyon_Kutuphanesi
{
    public class Kategoriler
    {
        public int Kategori_ID { get; set; }

        public string Kategori_AD { get; set; }


         bool available { get; set; }
        private void AllParameters(SqlCommand cmd)
        {

            if (this.Kategori_AD == null) cmd.Parameters.AddWithValue("@Kategori_AD", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Kategori_AD", this.Kategori_AD.Trim());

         
        }

        private Kategoriler FillProperty(SqlDataReader dr)
        {
            Kategoriler kate = new Kategoriler();
            kate.Kategori_ID = Convert.ToInt32(dr["Kategori_ID"]);
            if (dr["Kategori_AD"] != DBNull.Value) kate.Kategori_AD = dr["Kategori_AD"].ToString();

            kate.available = true;
            return kate;

        }
        public Kategoriler Find(int Kategori_ID)
        {
            VT vt = new VT();
            Kategoriler header = new Kategoriler();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Kategoriler where Kategori_ID=@Kategori_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Kategori_ID", Kategori_ID);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                header = FillProperty(dr);
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return header;
        }

        public List<Kategoriler> KategoriGetir()
        {
            List<Kategoriler> kategorigetir = new List<Kategoriler>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Kategoriler ", vt.baglanti);
     
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    kategorigetir.Add(FillProperty(dr));

                }
            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return kategorigetir;
        }



    }

}
