using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DictionaryTest
{
    public class DBHelper
    {
        //create database with specified name
        //the path should usually be Windows.Storage.ApplicationData.Current.LocalFolder
        public bool CreateDatabase(string DB_NAME)
        {
            if(!CheckFileExists(DB_NAME).Result)
            {
                string DB_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DB_NAME);

                using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DB_PATH))
                {
                    App.DB_PATH = DB_PATH;
                    conn.CreateTable<Definition>();

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        //check if the file filename exists
        private async Task<bool> CheckFileExists(string filename)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Insert new definition into the Definition table
        public void Insert(Definition def)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                conn.RunInTransaction(() =>
                {
                    conn.Insert(def);
                });
            }
        }

        //Retrieve a definition from the database by id
        public Definition GetDefinition(int defID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                var def = conn.Query<Definition>("SELECT * FROM Definition WHERE id =" + defID).FirstOrDefault();
                return def;
            }
        }

        //Return a list of all definitions in the database
        public ObservableCollection<Definition> GetAllDefinitions()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
                {
                    List<Definition> defs = conn.Table<Definition>().ToList<Definition>();
                    ObservableCollection<Definition> defList = new ObservableCollection<Definition>(defs);

                    return defList;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //Update existing definition in the database
        public void UpdateDefinition(Definition def)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                var existingDef = conn.Query<Definition>("SELECT * FROM Definition WHERE id =" + def.id).FirstOrDefault();

                if(existingDef != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Update(def);
                    });
                }
            }
        }

        //Clear the database of all definitions
        public void DeleteAllDefinitions()
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                conn.DropTable<Definition>();
                conn.CreateTable<Definition>();
                conn.Dispose();
                conn.Close();
            }
        }

        //Delete specific definition from the database
        public void DeleteDefinition(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), App.DB_PATH))
            {
                var existingDef = conn.Query<Definition>("SELECT * FROM Definition where id =" + id).FirstOrDefault();

                if(existingDef != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(existingDef);
                    });
                }
            }
        }



    }
}
