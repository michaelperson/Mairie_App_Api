using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.DAL.Configuration
{
    public class DatabaseConfiguration
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur reçevant la connection string
        /// </summary>
        /// <param name="connectionString">Connection string transmise par notre application principale</param>
        /// <exception cref="ArgumentNullException">Si la connection est null, cette exception est lancée</exception>
        public DatabaseConfiguration(string connectionString)
        {
            _connectionString = connectionString?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Créé une connection basée sur la connection string transmise lors de la constrcution
        /// </summary>
        /// <returns>Un objet de type <seealso cref="IDbConnection"/></returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}
