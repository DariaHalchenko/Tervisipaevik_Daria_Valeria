using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisipaevik_Daria_Valeria.Models
{
    [Table("Toidukorrad")]
    public class ToidukorradClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Toidukorrad_id { get; set; }
        public DateTime Kuupaev { get; set; }
        public TimeSpan Kallaaeg { get; set; }
        public string Roa_nimi { get; set; }
        public string Soogi_aeg { get; set; }
        public int Kalorid { get; set; }
    }
}
