using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Core.SubEntities
{
    public abstract class BaseEntity<T> : IEntity
    {
        [Key]
        [Display(Name = "شماره")]
       
        public T Id { get; set; }
    }
    public abstract class BaseEntity : BaseEntity<long>
    {

    }

    public static class BaseEntityService
    {
        public static IEnumerable<Type> GetModelsByAttribute(Type attribute)
        {


            Assembly assembly = Assembly.GetExecutingAssembly();

            // همه‌ی تایپ‌های موجود در Assembly را دریافت می‌کنیم
            Type[] types = assembly.GetTypes();

            // لیستی برای نگهداری تایپ‌هایی که دارای Custom Attribute مورد نظر هستند
            List<Type> typesWithCustomAttribute = new List<Type>();


            var typess = from assemblyy in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assemblyy.GetTypes()
                         where type.IsDefined(attribute)
                         select type;


            // بررسی Custom Attribute هر تایپ
            foreach (Type type in typess)
            {
                // بررسی Custom Attribute هر تایپ
                var attributes = type.GetCustomAttributes(attribute, false);
                if (attributes.Length > 0)
                {
                    // این تایپ دارای Custom Attribute مورد نظر است
                    typesWithCustomAttribute.Add(type);
                }
            }

            // چاپ نتیجه‌ی تایپ‌های دارای Custom Attribute مورد نظر

            //-----------------------------------------------------------------------
            //var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //            from type in assembly.GetTypes()
            //            where type.IsDefined(typeof(TableAttribute))
            //            select type;
            return typesWithCustomAttribute;


            var ass = typeof(BaseEntity).Assembly;
            var all = ass.GetExportedTypes().Where(c => typeof(BaseEntity).IsAssignableFrom(c));
            var aa = ass.GetExportedTypes().Where(c => c.IsClass && c.IsPublic && !c.IsAbstract && typeof(BaseEntity).IsAssignableFrom(c));
            return aa;

            // Assembly فعلی را دریافت می‌کنیم

        }
    }

}
