using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Adisyon_Kutuphanesi
{
    public class iletisim
    {
        public int Iletisim_ID { get; set; }
        public string Iletisim_Sabit_Tel { get; set; }
        public string Iletisim_Cep_Tel { get; set; }
        public string Iletisim_Mail { get; set; }
        public string Iletisim_il { get; set; }
        public string Iletisim_ilce { get; set; }
        public string Iletisim_Mahalle { get; set; }
        public string Iletisim_Sokak { get; set; }
        public int Iletisim_Bina_No { get; set; }
        public string hata { get; set; }
        public int periletisim_ID;
        public bool available { get; set; }
        

        private iletisim FillProperty(SqlDataReader dr)
        {
            iletisim islem = new iletisim();
            islem.Iletisim_ID = Convert.ToInt32(dr["Iletisim_ID"]);
            if (dr["Iletisim_Sabit_Tel"] != DBNull.Value) islem.Iletisim_Sabit_Tel = dr["Iletisim_Sabit_Tel"].ToString();
            if (dr["Iletisim_Cep_Tel"] != DBNull.Value) islem.Iletisim_Cep_Tel = dr["Iletisim_Cep_Tel"].ToString();
            if (dr["Iletisim_Mail"] != DBNull.Value) islem.Iletisim_Mail = dr["Iletisim_Mail"].ToString();
            if (dr["Iletisim_il"] != DBNull.Value) islem.Iletisim_il = dr["Iletisim_il"].ToString();
            if (dr["Iletisim_ilce"] != DBNull.Value) islem.Iletisim_ilce = dr["Iletisim_ilce"].ToString();
            if (dr["Iletisim_Mahalle"] != DBNull.Value) islem.Iletisim_Mahalle = dr["Iletisim_Mahalle"].ToString();
            if (dr["Iletisim_Sokak"] != DBNull.Value) islem.Iletisim_Sokak = dr["Iletisim_Sokak"].ToString();
            if (dr["Iletisim_Bina_No"] != DBNull.Value) islem.Iletisim_Bina_No = Convert.ToInt32(dr["Iletisim_Bina_No"]);
            islem.available = true;
            return islem;

        }

        public iletisim Find(int ID)
        {
            VT vt = new VT();
            iletisim header = new iletisim();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            SqlCommand cmd = new SqlCommand("select * from Tbl_iletisim where Iletisim_ID=@ID", vt.baglanti);
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

        private void AllParameters(SqlCommand cmd)
        {

            if (this.Iletisim_Sabit_Tel == null) cmd.Parameters.AddWithValue("@Iletisim_Sabit_Tel", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Sabit_Tel", this.Iletisim_Sabit_Tel.Trim());

            if (this.Iletisim_Cep_Tel == null) cmd.Parameters.AddWithValue("@Iletisim_Cep_Tel", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Cep_Tel", this.Iletisim_Cep_Tel.Trim());

            if (this.Iletisim_Mail == null) cmd.Parameters.AddWithValue("@Iletisim_Mail", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Mail", this.Iletisim_Mail.Trim());

            if (this.Iletisim_il == null) cmd.Parameters.AddWithValue("@Iletisim_il", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_il", this.Iletisim_il);

            if (this.Iletisim_ilce == null) cmd.Parameters.AddWithValue("@Iletisim_ilce", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_ilce", this.Iletisim_ilce);

            if (this.Iletisim_Mahalle == null) cmd.Parameters.AddWithValue("@Iletisim_Mahalle", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Mahalle", this.Iletisim_Mahalle.Trim());

            if (this.Iletisim_Sokak == null) cmd.Parameters.AddWithValue("@Iletisim_Sokak", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Sokak", this.Iletisim_Sokak.Trim());

            if (this.Iletisim_Bina_No == null) cmd.Parameters.AddWithValue("@Iletisim_Bina_No", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Iletisim_Bina_No", this.Iletisim_Bina_No);


        }

        public iletisim iletisimkayit(iletisim iletisim)
        {

            iletisim ilet = new iletisim();
            
            VT vt = new VT();

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();


            try
            {

                SqlCommand cmdekle = new SqlCommand("insert  into Tbl_iletisim (Iletisim_Sabit_Tel,Iletisim_Cep_Tel,Iletisim_Mail,Iletisim_il,Iletisim_ilce,Iletisim_Mahalle,Iletisim_Sokak,Iletisim_Bina_No) output inserted.Iletisim_ID Values(@Sabit_Tel,@Cep_Tel,@Mail,@il,@ilce,@Mahalle,@Sokak,@no)", vt.baglanti);
                cmdekle.Parameters.AddWithValue("@Sabit_Tel", iletisim.Iletisim_Sabit_Tel);
                cmdekle.Parameters.AddWithValue("@Cep_Tel", iletisim.Iletisim_Cep_Tel);
                cmdekle.Parameters.AddWithValue("@Mail", iletisim.Iletisim_Mail);
                cmdekle.Parameters.AddWithValue("@il", iletisim.Iletisim_il);
                cmdekle.Parameters.AddWithValue("@ilce", iletisim.Iletisim_ilce);
                cmdekle.Parameters.AddWithValue("@Mahalle", iletisim.Iletisim_Mahalle);
                cmdekle.Parameters.AddWithValue("@Sokak", iletisim.Iletisim_Sokak);
                cmdekle.Parameters.AddWithValue("@no", iletisim.Iletisim_Bina_No);

                periletisim_ID = Convert.ToInt32(cmdekle.ExecuteScalar());
                iletisim.hata = "Yeni İletişim Bilgisi Oluşturuldu";
            }
            catch (Exception exception) { iletisim.hata = exception.ToString(); }


            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();

            return iletisim;


        }
       

        public DataTable listeleguncelle(int personel_id)
        {
           
            VT vt = new VT();
             SqlDataAdapter iletisimdatacmd;
         DataSet iletisimds;
         SqlCommandBuilder iletisimcmdb;

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            iletisimdatacmd = new SqlDataAdapter("select Tbl_iletisim.* from Tbl_iletisim  join Tbl_Personel  ON Tbl_iletisim.Iletisim_ID=Tbl_Personel.Personel_Iletisim_ID where Tbl_Personel.Personel_ID="+personel_id, vt.baglanti);

             

            iletisimcmdb = new SqlCommandBuilder(iletisimdatacmd);
            iletisimds = new DataSet();
            iletisimdatacmd.Fill(iletisimds, "Tbl_iletisim");
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return iletisimds.Tables[0];

        }

        public iletisim iletisimguncelle(iletisim ilet,int id)
        {
            VT vt = new VT();
            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();
            SqlCommand cmd = new SqlCommand(@"UPDATE Tbl_iletisim SET  
                                                            Iletisim_Sabit_Tel=@Iletisim_Sabit_Tel,
                                                            Iletisim_Cep_Tel=@Iletisim_Cep_Tel,
                                                            Iletisim_Mail=@Iletisim_Mail,
                                                            Iletisim_il=@Iletisim_il,
                                                            Iletisim_ilce=@Iletisim_ilce,
                                                            Iletisim_Mahalle=@Iletisim_Mahalle ,
                                                            Iletisim_Sokak=@Iletisim_Sokak,
                                                            Iletisim_Bina_No=@Iletisim_Bina_No
                                                            WHERE Iletisim_ID=@ID", vt.baglanti);

            cmd.Parameters.AddWithValue("@ID", id);
            AllParameters(cmd);
            cmd.ExecuteNonQuery();
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return ilet;

        }

        public DataTable MusteriListele(int musteri_id)
        {

            VT vt = new VT();
            SqlDataAdapter iletisimdatacmd;
            DataSet iletisimds;
            SqlCommandBuilder iletisimcmdb;

            if (vt.baglanti.State == ConnectionState.Closed) vt.baglanti.Open();

            iletisimdatacmd = new SqlDataAdapter("select Tbl_iletisim.* from Tbl_iletisim  join Tbl_Musteriler  ON Tbl_iletisim.Iletisim_ID=Tbl_Musteriler.Musteri_iletisim_ID where Tbl_Musteriler.Musteri_ID=" + musteri_id, vt.baglanti);
            iletisimcmdb = new SqlCommandBuilder(iletisimdatacmd);
            iletisimds = new DataSet();
            iletisimdatacmd.Fill(iletisimds, "Tbl_iletisim");
            if (vt.baglanti.State == ConnectionState.Open) vt.baglanti.Close();
            return iletisimds.Tables[0];

        }

    }
}
