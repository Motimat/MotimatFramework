using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mf.Core.Interfaces;
using Mf.Core.SubEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.EfCore.Services
{
    public class SqlServerContextCore : DbContext, IContextCore
    {
        public SqlServerContextCore(DbContextOptions optinos) : base(optinos) { }

        public List<Type>? Entities { get; set; }

        public IGenericService<TypeModel> GetService<TypeModel>() where TypeModel : class, IEntity
        {
            var telBotServiceType = typeof(EntityFrameworkCoreServices<>).MakeGenericType(typeof(TypeModel));
            var constructor = telBotServiceType.GetConstructor(new[] { typeof(SqlServerContextCore) });
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
            var constructor = telBotServiceType.GetConstructor(new[] { typeof(SqlServerContextCore) });
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



        public List<object> GetGenericServices(List<Type> entities)
        {
            List<object> services = new List<object>();
            foreach (var type in entities)
            {
                var telBotServiceType = typeof(EntityFrameworkCoreServices<>).MakeGenericType(type);
                // Get the constructor of the TelBot_Service<T> type
                var constructor = telBotServiceType.GetConstructor(new[] { typeof(SqlServerContextCore) });
                if (constructor != null)
                {
                    // If constructor exists, create an instance with a parameter
                    var telBotServiceInstance = constructor.Invoke(new object[] { this }); // Provide the parameter value here
                    //var ssss = (IGenericService<IEntity>)telBotServiceInstance;
                    //var sss = (EntityFrameworkCoreServices<IEntity>)telBotServiceInstance;
                    services.Add(telBotServiceInstance);
                }
                else
                {
                    // If no constructor exists, create an instance without parameters
                    var telBotServiceInstance = Activator.CreateInstance(telBotServiceType);
                    //var sss = (EntityFrameworkCoreServices<IEntity>)telBotServiceInstance;
                    services.Add(telBotServiceInstance);
                }
            }
            return services;
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Entities != null)
            {
                foreach (var s in Entities)
                {
                    var entity = modelBuilder.Entity(s);
                }


                //for postgres db
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime))
                        {
                            property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),   // ذخیره
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)              // بازیابی
                            ));
                        }

                        else if (property.ClrType == typeof(DateTime?))
                        {
                            property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()) : v, // ذخیره
                                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v                     // بازیابی
                            ));
                        }
                    }
                }
            }
        }
        void IContextCore.Dispose()
        {
            this.Dispose();
        }

        public void Migrate()
        {
            this.Migrate();
        }
    }
}
