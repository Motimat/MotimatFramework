using Mf.Core.SubEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mf.Core.SharedKernel;

namespace Mf.Core.Interfaces
{
    public interface IContextCore
    {
        public IGenericService<TEntity> GetService<TEntity>() where TEntity : BaseEntity;

        public object GetService(Type type);

        public void Dispose();

        public void Migrate();
    }
}
