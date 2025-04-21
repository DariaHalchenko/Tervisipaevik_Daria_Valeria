using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.Database
{
    public class ToidukorradDatabase
    {
        SQLiteConnection db;

        public ToidukorradDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<ToidukorradClass>();
        }

        public IEnumerable<ToidukorradClass> GetToidukorrad()
        {
            return db.Table<ToidukorradClass>().ToList();
        }

        public ToidukorradClass GetToidukorrad(int id)
        {
            return db.Get<ToidukorradClass>(id);
        }

        public int SaveToidukorrad(ToidukorradClass toidukorrad)
        {
            if (toidukorrad.Toidukorrad_id != 0)
            {
                db.Update(toidukorrad);
                return toidukorrad.Toidukorrad_id;
            }
            else
            {
                return db.Insert(toidukorrad);
            }
        }

        public int DeleteToidukorrad(int id)
        {
            return db.Delete<ToidukorradClass>(id);
        }
    }
}

