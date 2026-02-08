using Microsoft.EntityFrameworkCore;
using Mf.Core.Interfaces;
using Mf.Core.SubEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.EfCore.Services
{
    public abstract class SqliteContextCore : DbContext, IContextCore
    {
        public string DbName { get; set; }
        public string DbPath { get; set; }

        public List<Type> Entities { get; set; }

        public void SetDefaultConfig()
        {
            DbName = "defualt-database-name.db";
            DbPath = "";
        }
        public SqliteContextCore(bool setDefaultConfig = true)
        {

            if (setDefaultConfig)
            {
                SetDefaultConfig();
                CreateDB();
            }

        }
        public void CreateDB()
        {
            //TODO clean this
            //var strDestination = "copy.db";
            //string dir = Directory.GetCurrentDirectory();
            //dir = Path.Combine(dir, strDestination);


            //string orgDir = Path.Combine(Directory.GetCurrentDirectory(), DbPath, DbName);

            //if (!Path.Exists(Path.Combine(Directory.GetCurrentDirectory(), DbPath)))
            //{
            //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), DbPath));
            //}

            //if (!File.Exists(orgDir))
            //{

            //    System.Data.SQLite.SQLiteConnection.CreateFile(orgDir);
            //}

            //if (!File.Exists(dir))
            //{

            //    System.Data.SQLite.SQLiteConnection.CreateFile(dir);
            //}

            //using (var location = new System.Data.SQLite.SQLiteConnection(string.Format(@"Data Source={0};", orgDir)))
            ////using (var destination = new System.Data.SQLite.SQLiteConnection(string.Format(@"Data Source={0}", dir)))
            //{
            //    location.Open();
            //    //destination.Open();
            //    //location.BackupDatabase(destination, "main", "main", -1, null, 0);
            //    //destination.Close();
            //}



        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}{DbName}");
        }


        public IGenericService<TypeModel> GetService<TypeModel>() where TypeModel : class, IEntity
        {
            var telBotServiceType = typeof(EntityFrameworkCoreServices<>).MakeGenericType(typeof(TypeModel));
            var constructor = telBotServiceType.GetConstructor(new[] { typeof(SqliteContextCore) });
            if (constructor != null)
            {
                var telBotServiceInstance = constructor.Invoke(new object[] { this });
                return (EntityFrameworkCoreServices<TypeModel>)telBotServiceInstance;
            }
            else
            {

                var telBotServiceInstance = Activator.CreateInstance(telBotServiceType);

                return (EntityFrameworkCoreServices<TypeModel>)telBotServiceInstance!;
            }
        }

        public object GetService(Type type)
        {
            var telBotServiceType = typeof(EntityFrameworkCoreServices<>).MakeGenericType(type);
            var constructor = telBotServiceType.GetConstructor(new[] { typeof(SqliteContextCore) });
            if (constructor != null)
            {
                var telBotServiceInstance = constructor.Invoke(new object[] { this });
                return telBotServiceInstance;
            }
            else
            {
                var telBotServiceInstance = Activator.CreateInstance(telBotServiceType);
                return telBotServiceInstance!;
            }
        }

        void IContextCore.Dispose()
        {
            this.Dispose();
        }

        public void Migrate()
        {
            try
            {
                this.Migrate();
            }
            catch
            {

            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Entities != null)
            {
                foreach (var s in Entities)
                {
                    var entity = modelBuilder.Entity(s);
                }
            }
        }

       
    }
}
