using System;
using System.Collections.Generic;
using System.Text;

namespace Mf.Core.Interfaces
{
    public interface ICommand
    {

    }

    public interface ICreateCommand<TEntity>: ICommand
    {

    }

    public interface IUpdateCommand<TEntity> : ICommand
    {

    }

    public interface IDeleteCommand<TEntity> : ICommand
    {

    }
}
