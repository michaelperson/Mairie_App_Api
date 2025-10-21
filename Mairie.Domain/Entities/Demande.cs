using Mairie.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mairie.Domain.Entities
{
    /// <summary>
    /// Entitée représentant un enregistrement de la table Demande
    /// </summary>
    public class Demande
    {
        public int Id { get; set; }
        public string NomCitoyen { get; set; } = string.Empty;
        public string TypeDemande { get; set; } = string.Empty;
        public StatutEnum Statut {  get; set; } = 0;
        public DateTime DateCreation { get;set; }
    }
}
