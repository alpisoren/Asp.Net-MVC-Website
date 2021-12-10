using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Adisyon_Kutuphanesi;

namespace AdisyonProje
{
    public partial class Frm_Giris : DevExpress.XtraEditors.XtraForm
    {
        public Frm_Giris()
        {
            InitializeComponent();
        }

        private void btn_giris_Click(object sender, EventArgs e)
        {
            Personel per = new Personel();
           per= per.PersonelGiris(txt_giris_kuladi.Text.ToString(),txt_giris_sifre.Text.ToString());
            Frm_Anasayfa anasayfa = new Frm_Anasayfa();
            
            anasayfa.Per_pozisyon = per.Personel_Pozisyon;
            anasayfa.Per_ID = per.Personel_ID;
            anasayfa.Per_Ad = per.Personel_Ad;
            if (per.Personel_Ad != "Yanlış !!") { this.Hide(); anasayfa.Show(); }


        }
    }
}