using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<string>> GetRolesByWindowsIdAsync(string windowsId);

        Task<string?> GetDepartementByWindowsIdAsync(string windowsId);
    }
}
