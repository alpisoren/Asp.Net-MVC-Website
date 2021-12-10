using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adisyon_Kutuphanesi
{
    public class Personel
    {
        public int Personel_ID { get; set; }
        public string Personel_Ad { get; set; }
        public string Personel_Soyad { get; set; }
        public string Personel_TC { get; set; }
        public double Personel_Maas { get; set; }
        public DateTime Personel_IseGiris { get; set; }
        public string Personel_Pozisyon { get; set; }
        public int Personel_Iletisim_ID { get; set; }
        public string Personel_KullaniciAdi { get; set; }
        public string Personel_Sifre { get; set; }
        public string hata;
        public bool available { get; set; }
        
        public iletisim iletisimgetir = new iletisim();

        private void AllParameters(SqlCommand cmd)
        {

            if (this.Personel_Ad == null) cmd.Parameters.AddWithValue("@Personel_Ad", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_Ad", this.Personel_Ad.Trim());

            if (this.Personel_Soyad == null) cmd.Parameters.AddWithValue("@Personel_Soyad", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_Soyad", this.Personel_Soyad.Trim());

            if (this.Personel_TC == null) cmd.Parameters.AddWithValue("@Personel_TC", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_TC", this.Personel_TC.Trim());

            if (this.Personel_Maas == null) cmd.Parameters.AddWithValue("@Personel_Maas", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_Maas", this.Personel_Maas);

            if (this.Personel_IseGiris == null) cmd.Parameters.AddWithValue("@Personel_IseGiris", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_IseGiris", this.Personel_IseGiris);

            if (this.Personel_Pozisyon == null) cmd.Parameters.AddWithValue("@Personel_Pozisyon", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_Pozisyon", this.Personel_Pozisyon.Trim());

            if (this.Personel_KullaniciAdi == null) cmd.Parameters.AddWithValue("@Personel_KullaniciAdi", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_KullaniciAdi", this.Personel_KullaniciAdi.Trim());

            if (this.Personel_Sifre == null) cmd.Parameters.AddWithValue("@Personel_Sifre", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Personel_Sifre", this.Personel_Sifre.Trim());


        }

        public bool PersonelKullaniciAdiKontrol(string kuladi)
        {
            bool sonuc=false;
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmdkontrol = new SqlCommand("select  * from Tbl_Personel   where Personel_KullaniciAdi=@personelkul  ", vt.baglanti);
            cmdkontrol.Parameters.AddWithValue("@personelkul", kuladi);//ayrı bi metod yap

            SqlDataReader drkontrol = cmdkontrol.ExecuteReader();
            if (drkontrol.HasRows)
            {

                sonuc = true;
                drkontrol.Close();
                drkontrol.Dispose();


            }
            else
            {
                sonuc = false;
                drkontrol.Close();
                drkontrol.Dispose();

            }
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return sonuc;
        }
        public Personel PersonelGiris(string KullaniciAdi, string Sifre)
        {
            Personel personel = new Personel();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Personel where Personel_KullaniciAdi=@KullaniciAdi and Personel_Sifre=@Sifre AND Personel_Durum='True'", vt.baglanti);
            cmd.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi.Trim());
            cmd.Parameters.AddWithValue("@Sifre", Sifre.Trim());

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    personel.Personel_Ad = dr["Personel_Ad"].ToString();
                    personel.Personel_Pozisyon = dr["Personel_Pozisyon"].ToString();
                    personel.Personel_ID = Convert.ToInt32(dr["Personel_ID"]);
                }
            }
            else { personel.Personel_Ad = "Yanlış !!"; }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return personel;
        }

        public Personel PersonelKayit(Personel per)
        {
            Personel personel = new Personel();
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            
            try
            {
                if (PersonelKullaniciAdiKontrol(per.Personel_KullaniciAdi) == false)
                {

                    iletisimgetir = iletisimgetir.iletisimkayit(iletisimgetir);
                    SqlCommand cmdekle = new SqlCommand("insert  into Tbl_Personel (Personel_Ad,Personel_Soyad,Personel_TC,Personel_Maas,Personel_IseGiris,Personel_Pozisyon,Personel_Iletisim_ID,Personel_KullaniciAdi,Personel_Sifre,Personel_Durum) Values(@Personel_Ad,@Personel_Soyad,@Personel_TC,@Personel_Maas,@Personel_IseGiris,@Personel_Pozisyon,@personeliletisim,@Personel_KullaniciAdi,@Personel_Sifre,'True')", vt.baglanti);
                    AllParameters(cmdekle);
                    cmdekle.Parameters.AddWithValue("@personeliletisim", iletisimgetir.periletisim_ID);
                    cmdekle.ExecuteNonQuery();
                    personel.hata = "Personel Kaydı Başarıyla Yapıldı.";

                }
                else { personel.hata = "Personel Kaydı Mevcut."; }
              
                
            }
            catch(Exception ex) { personel.hata = ex.ToString(); }

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return personel;
        }

      
        public List<Personel> PersonelGetir()
        {
            List<Personel> Personeller = new List<Personel>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Personel Where Personel_Durum='True'", vt.baglanti);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Personeller.Add(new Personel
                    {
                      
                        Personel_ID = Convert.ToInt32(dr["Personel_ID"]),//Bunları düzenle personel sielrken ıd alınamıyor
                        Personel_Ad = dr["Personel_Ad"].ToString(),
                        Personel_Soyad = dr["Personel_Soyad"].ToString(),
                        Personel_TC = dr["Personel_TC"].ToString(),
                      // Personel_Maas = Convert.ToDouble(dr["Personel_Maas"]),
                     //  Personel_IseGiris = Convert.ToDateTime(dr["Personel_IseGiris"]),
                        Personel_Pozisyon = dr["Personel_Pozisyon"].ToString(),
                      //  Personel_Iletisim_ID = Convert.ToInt32(dr["Personel_Iletisim_ID"]),
                    //    Personel_KullaniciAdi = dr["Personel_KullaniciAdi"].ToString(),
                     //   Personel_Sifre = dr["Personel_Sifre"].ToString()

                    });
                }
            }

            return Personeller;
        }//SIKINTILI

        public Personel PersonelCikar(int id)
        {
            Personel per = new Personel();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmddurum = new SqlCommand("Update  Tbl_Personel SET Personel_Durum='False' Where Personel_ID=@KitapDurumDeisId", vt.baglanti);
            cmddurum.Parameters.AddWithValue("@KitapDurumDeisId", id);

            cmddurum.ExecuteNonQuery();

            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return per;
        }

        public List<Personel> EskiPersonelGetir()
        {
            List<Personel> Personeller = new List<Personel>();
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand("select * from Tbl_Personel Where Personel_Durum='False'", vt.baglanti);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Personeller.Add(new Personel
                    {
                        Personel_ID = dr.GetInt32(0),
                        Personel_Ad = dr.GetString(1),
                        Personel_Soyad = dr.GetString(2),
                        Personel_TC = dr.GetString(3)

                    });
                }
            }

            return Personeller;
        }

        public Personel guncelleme(Personel per,int id)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_Personel SET  
                                                            Personel_Ad=@Personel_Ad,
                                                            Personel_Soyad=@Personel_Soyad,
                                                            Personel_TC=@Personel_TC,
                                                            Personel_Maas=@Personel_Maas,
                                                            Personel_Pozisyon=@Personel_Pozisyon,
                                                            Personel_KullaniciAdi=@Personel_KullaniciAdi,
                                                            Personel_Sifre=@Personel_Sifre 
                                                            WHERE Personel_ID=@ID", vt.baglanti);
       
            cmd.Parameters.AddWithValue("@ID", id);
            AllParameters(cmd);
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return per;
        }

        public DataTable listeleguncelle()
        {
            Personel personeller = new Personel();
            VT vt = new VT();
            SqlDataAdapter datacmd;
            DataSet ds;
            SqlCommandBuilder cmdb;

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            datacmd = new SqlDataAdapter("select   Personel_ID,Personel_Ad, Personel_Soyad ,Personel_TC ,Personel_Maas,   Personel_Pozisyon ,Personel_KullaniciAdi,Personel_Sifre from Tbl_Personel Where Personel_Durum='True'", vt.baglanti);



            cmdb = new SqlCommandBuilder(datacmd);
            ds = new DataSet();
            datacmd.Fill(ds, "Tbl_Personel");
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return ds.Tables[0];

        }


        private Personel FillProperty(SqlDataReader dr)
        {
            Personel islem = new Personel();
            islem.Personel_ID = Convert.ToInt32(dr["Personel_ID"]);
            if (dr["Personel_Ad"] != DBNull.Value) islem.Personel_Ad =dr["Personel_Ad"].ToString();
            if (dr["Personel_Soyad"] != DBNull.Value) islem.Personel_Soyad = dr["Personel_Soyad"].ToString();
            if (dr["Personel_TC"] != DBNull.Value) islem.Personel_TC = dr["Personel_TC"].ToString();
            if (dr["Personel_KullaniciAdi"] != DBNull.Value) islem.Personel_KullaniciAdi = dr["Personel_KullaniciAdi"].ToString();
            if (dr["Personel_Sifre"] != DBNull.Value) islem.Personel_Sifre = dr["Personel_Sifre"].ToString();
            if (dr["Personel_Pozisyon"] != DBNull.Value) islem.Personel_Pozisyon = dr["Personel_Pozisyon"].ToString();
            if (dr["Personel_Maas"] != DBNull.Value) islem.Personel_Maas = Convert.ToInt32(dr["Personel_Maas"]);
            islem.available = true;
            return islem;

        }
        public Personel Find(int ID)
        {
            VT vt = new VT();
            Personel header = new Personel();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_Personel where Personel_ID=@ID", vt.baglanti);
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
    }
}
