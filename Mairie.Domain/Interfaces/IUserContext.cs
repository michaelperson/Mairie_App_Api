using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.Domain.Interfaces
{
    public interface IUserContext
    {
        string WindowsId { get; }
        string? DisplayName { get; }
        string AccountName { get; }
        bool IsAuthenticated { get; }
    }
}
