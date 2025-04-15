using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisipaevik_Daria_Valeria.Models
{
    [Table("Treeningud")]
    public class TreeningudClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Treeningud_id { get; set; }
        public string Treeningu_nimi { get; set; }
        public DateTime Kuupaev { get; set; }
        public TimeSpan Kallaaeg { get; set; }
        public string Treeningu_tuup { get; set; }
        public int Sammude_Arv { get; set; }
        public int Kulutud_kalorid { get; set; }
    }
}
