using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisipaevik_Daria_Valeria.Models
{
    [Table("Veejalgimine")]
    public class VeejalgimineClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Veejalgimine_id { get; set; }
        public DateTime Kuupaev { get; set; }
        public int Kogus { get; set; }
        public bool Aktiivne { get; set; }
    }
}
