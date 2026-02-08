using System;
using System.Collections.Generic;
using System.Text;

namespace Mf.Razor.Entity.Interfaces
{
    public interface ITokenService
    {
        Task<string?> GetToken();
        Task<bool> SetToken(string token);
        Task<bool> RemoveToke();
    }
}
