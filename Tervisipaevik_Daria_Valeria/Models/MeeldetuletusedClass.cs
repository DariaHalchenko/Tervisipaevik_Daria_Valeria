using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisipaevik_Daria_Valeria.Models
{
    [Table("Meeldetuletused")]
    public class MeeldetuletusedClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Meeldetuletused_id { get; set; }
        public string tuup { get; set; }
        public DateTime Kuupaev { get; set; }
        public TimeSpan Kallaaeg { get; set; }
        public bool Aktiivne { get; set; }
    }
}
