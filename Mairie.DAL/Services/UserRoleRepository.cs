using Dapper;
using Mairie.DAL.Configuration;
using Mairie.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.DAL.Services
{
    public class UserRoleRepository : BaseRepository, IUserRoleRepository
    {
        public UserRoleRepository(DatabaseConfiguration dbConfig) : base(dbConfig) { }

        public async Task<string?> GetDepartementByWindowsIdAsync(string windowsId)
        {
            string query = "Select Departement FROM UserRoles WHERE WindowsId= @WinId";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { WinId = windowsId });
        }

        public async Task<IEnumerable<string>> GetRolesByWindowsIdAsync(string windowsId)
        {
            string query = "Select Role FROM UserRoles WHERE WindowsId= @WinId";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryAsync<string>(query, new { WinId = windowsId });
        }
    }
}
