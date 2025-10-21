using Dapper;
using Mairie.DAL.Configuration;
using Mairie.Domain.Entities;
using Mairie.Domain.Enumerations;
using Mairie.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.DAL.Services
{
    public class DemandeRepository : BaseRepository, IDemandeRepository
    {
        public DemandeRepository(DatabaseConfiguration dbConfig) : base(dbConfig) { }

 
        public async Task<int> CreateAsync(Demande demande)
        {
            //Version ORacle
            //string sql = @"INSERT INTO Demande  
            //                (NomCitoyen, TypeDEmande, Statut, DateCreation)
            //                VALUES
            //                (@NomCitoyen, @TypeDEmande, @Statut, @DateCreation)
            //                RETURNING Id";


            string sql = @"INSERT INTO Demande 
                            (NomCitoyen, TypeDEmande, Statut, DateCreation)
                            OUTPUT INSERTED.Id
                            VALUES
                            (@NomCitoyen, @TypeDEmande, @Statut, @DateCreation)";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { 
               demande.NomCitoyen, demande.TypeDemande, demande.Statut, DateCreation= demande.DateCreation
            });
                                
        }

        public async Task<bool> DeleteAsync(int id)
        {
            //
            //Demande? demande = await GetByIdAsync(id);
            //if (demande == null) return false;
            string query = @"DELETE
                             FROM Demande 
                             WHERE Id=@Id";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.ExecuteAsync(query, new { Id = id }) > 0;
        }

        public async Task<IEnumerable<Demande>> GetAllAsync()
        {
            string query = @"SELECT
                             Id, NomCitoyen, TypeDemande, Statut
                             FROM Demande 
                             ORDER BY DateCreation DESC";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryAsync<Demande>(query);
        }

        public async Task<Demande?> GetByIdAsync(int id)
        {
            string query = @"SELECT
                             Id, NomCitoyen, TypeDemande, Statut
                             FROM Demande 
                             WHERE Id= @Id
                             ORDER BY DateCreation DESC";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Demande>(query, new { Id= id });
        }

        public async Task<IEnumerable<Demande>> GetByStatusAsync(StatutEnum statut)
        {
            string query = @"SELECT
                             Id, NomCitoyen, TypeDemande, Statut
                             FROM Demande 
                             WHERE Statut= @statut
                             ORDER BY DateCreation DESC";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryAsync<Demande>(query, new { statut= statut.ToString() });
        }

        public async Task<IEnumerable<Demande>> SearchAsync(string searchTerm)
        {
            string query = @"SELECT
                             Id, NomCitoyen, TypeDemande, Statut
                             FROM Demande 
                             WHERE NomCitoyen LIKE @SearchPattern
                                   OR TypeDemande  LIKE @SearchPattern
                             ORDER BY DateCreation DESC";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.QueryAsync<Demande>(query, new { SearchPattern= $"%{searchTerm}%" });
        }

        public async Task<bool> UpdateAsync(Demande demande)
        {
            string query = @"UPDATE Demande
                             SET
                                NomCitoyen = @NomCitoyen,
                                TypeDemande = @TypeDemande,
                                Statut= @Statut
                             WHERE Id=@Id";
            using IDbConnection connection = _dbConfig.CreateConnection();
            return await connection.ExecuteAsync(query, demande) > 0; 
            //Remarque : Il faut normalement avoir le nombre de paramètre exacte et non pas
            //transmettre l'objet complet si toutes les propriétés ne sont pas utilisées.
            //Ici c'est pour un exemple.
        }
    }
}
