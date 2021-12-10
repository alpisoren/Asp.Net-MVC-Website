using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing;

namespace Adisyon_Kutuphanesi
{
    public class Masa
    {
        public int Masa_ID { get; set; }
        public byte Masa_Durum { get; set; }
        public byte Masa_Rez { get; set; }
        public bool masaanlik, masarez;
        private void AllParameters(SqlCommand cmd)
        {

            if (this.Masa_Durum == null) cmd.Parameters.AddWithValue("@Masa_Durum", 0);
            else cmd.Parameters.AddWithValue("@Masa_Durum", this.Masa_Durum);
            
            if (this.Masa_Rez == null) cmd.Parameters.AddWithValue("@Masa_Rez", 0);
            else cmd.Parameters.AddWithValue("@Masa_Rez", this.Masa_Rez);

        }

        public int Masaid()
        {
          

            List<string> masas = new List<string>();

            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Masalar", vt.baglanti);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    


                    masas.Add(dr["Masa_ID"].ToString());

                
                }
            }
            return masas.Count();

        }

        public Masa MasaEkle(string ad)
        {
            Masa m = new Masa();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Masalar (Masa_Durum,Masa_Rez) Values(default,default)", vt.baglanti);
      
            cmdekle.ExecuteNonQuery();


            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return m;
        }

        public Masa guncelleme(Masa mas, int id)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Masalar SET  
                                                            Masa_Durum=@Masa_Durum,
                                                            Masa_Rez=@Masa_Rez
                                                            WHERE Masa_ID=@ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@ID", id);
            AllParameters(cmd);
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return mas;
        }

        public bool Durumlar(int id)
        {
            List<Masa> MasaDurumlar = new List<Masa>();

            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select Masa_Durum from Tbl_Masalar Where Masa_ID=@Masa_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Masa_ID", id);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    masaanlik = Convert.ToBoolean(dr["Masa_Durum"]);

                }
            }


            return masaanlik;

        }

        public bool Rezler(int id)
        {
            List<Masa> MasaDurumlar = new List<Masa>();

            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select Masa_Rez from Tbl_Masalar Where Masa_ID=@Masa_ID", vt.baglanti);
            cmd.Parameters.AddWithValue("@Masa_ID", id);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    masarez = Convert.ToBoolean(dr["Masa_Rez"]);

                }
            }


            return masarez;

        }









        /*
        public void masaolusturyap(int id)
        {

            SimpleButton YeniButon = new SimpleButton();

            YeniButon.Name = id.ToString();
            btn = YeniButon.Name;
            YeniButon.Text = "Masa " + btn;
            YeniButon.Size = new Size(110, 40);
            YeniButon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;

            Masa m = new Masa();

            if (m.Durumlar(id) == true)
            {
                YeniButon.Appearance.BackColor = Color.Red;

                if (m.Rezler(id) == true)
                {
                    YeniButon.Text = "Masa " + btn + " (DOLU-REZ)";
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
                    YeniButon.Text = "Masa " + btn + " (Rezerveli)";
                }
                else
                {
                    YeniButon.Text = "Masa " + btn + " (BOŞ)";
                }
            }



            YeniButon.Location = new Point(60, 60);

            YeniButon.Click += new EventHandler(YeniButon_Click);
            flowLayoutPanel1.Controls.Add(YeniButon);
        }
        */

    }

}
