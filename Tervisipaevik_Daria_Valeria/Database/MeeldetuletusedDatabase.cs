using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.Database
{
    public class MeeldetuletusedDatabase
    {
        SQLiteConnection db;

        public MeeldetuletusedDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<MeeldetuletusedClass>();
        }
        public IEnumerable<MeeldetuletusedClass> GetMeeldetuletused()
        {
            return db.Table<MeeldetuletusedClass>().ToList();
        }

        public MeeldetuletusedClass GetMeeldetuletused(int id)
        {
            return db.Get<MeeldetuletusedClass>(id);
        }

        public int SaveMeeldetuletused(MeeldetuletusedClass meeldetuletused)
        {
            if (meeldetuletused.Meeldetuletused_id != 0)
            {
                db.Update(meeldetuletused);
                return meeldetuletused.Meeldetuletused_id;
            }
            else
            {
                return db.Insert(meeldetuletused);
            }
        }

        public int DeleteMeeldetuletused(int id)
        {
            return db.Delete<MeeldetuletusedClass>(id);
        }
    }
}
