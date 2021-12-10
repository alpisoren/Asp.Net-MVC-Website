using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace Adisyon_Kutuphanesi
{
   public class Saatler
    {
        public int Saat_ID { get; set; }
        public string Saat_Saat  { get; set; }
        public byte saatrez { get; set; }



        public int Saatid()
        {


            List<string> saatidler = new List<string>();

            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Saatler", vt.baglanti);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {



                    saatidler.Add(dr["Saat_ID"].ToString());


                }
            }
            return saatidler.Count();

        }
        public Saatler SaatGetir(Saatler saatler ,int Saat_ID)
        {
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Saatler Where Saat_ID=@Saat_ID  ", vt.baglanti);
            cmd.Parameters.AddWithValue("@Saat_ID", Saat_ID);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Saat_Saat = dr["Saat_Saat"].ToString();      
                }
            }

            return saatler;
        }
        public byte SaatDurum(int id , int Masa_ID, DateTime tarih)
        {
          

            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Rezervasyon Where Rez_Saat_ID=@Saat_ID AND Rez_Masa_ID=@Masa_ID AND Rez_Baslangic=@tarih AND Rez_Aktif=@aktif", vt.baglanti);
            cmd.Parameters.AddWithValue("@Saat_ID", id);
            cmd.Parameters.AddWithValue("@Masa_ID", Masa_ID);
            cmd.Parameters.AddWithValue("@tarih", tarih);
            cmd.Parameters.AddWithValue("@aktif", 1);
            saatrez=Convert.ToByte( cmd.ExecuteScalar());

          


            return saatrez;

        }

    }
}
