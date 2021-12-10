using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adisyon_Kutuphanesi
{
    public class VT
    {
        public SqlConnection baglanti = new SqlConnection(@"Data Source= DESKTOP-GRUUGOJ;Database=Adisyon_Otomasyon;Integrated Security=True");

    }
}
