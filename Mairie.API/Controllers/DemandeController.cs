using Mairie.Domain.DTOs;
using Mairie.Domain.Entities;
using Mairie.Domain.Enumerations;
using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations; 

namespace Mairie.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandeController : ControllerBase
    {
        private readonly IDemandeRepository _repository;
        private readonly ILogger<DemandeController> _logger;
       
        public DemandeController(IDemandeRepository demandeRepository, ILogger<DemandeController> logger )
        {
            _repository= demandeRepository;
            _logger = logger;
        }
        [HttpGet]
        [SwaggerResponse(200, "Liste des demandes", typeof(IEnumerable<DemandeReadDTO>))] 
        [SwaggerResponse(404, "Aucune demande trouvée")]
        [SwaggerResponse(500, "Erreur lors de la récupération des données")]
        
        public async Task<ActionResult<IEnumerable<DemandeReadDTO>>> GetAll()
        {
            try
            {
                IEnumerable<Demande> demandes = await _repository.GetAllAsync();
                if (demandes.Count() > 0) 
                    return Ok(
                    demandes.Select(
                                        d => new DemandeReadDTO()
                                        {
                                            Id = d.Id,
                                            NomCitoyen = d.NomCitoyen,
                                            Statut = d.Statut,
                                            TypeDemande = d.TypeDemande

                                        }                    
                    ));
                else return NotFound();
            }
            catch (Exception ex )
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes");
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
    Summary = "Récupère une demande par son identifiant",
    Description = "Retourne les informations détaillées d’une demande d'un citoyen de la mairie."
)]
        [SwaggerResponse(200, "La demande identifiée par l'id", typeof(DemandeReadDTO))]
        [SwaggerResponse(404, "Aucune demande trouvée avec l'id transmis")]
        [SwaggerResponse(500, "Erreur lors de la récupération des données")]
        public async Task<ActionResult<DemandeReadDTO>> GetById(
            [SwaggerParameter("Identifiant de la demande", Required = true)] 
            int id
            )
        {
            if(id<=0) return BadRequest("Id invalide");

            try
            {
                Demande? demande = await _repository.GetByIdAsync(id);
                if (demande == null) return NotFound($"Demande avec l'ID {id} introuvable");

                //Mapper
                DemandeReadDTO demandeReadDTO = new DemandeReadDTO()
                {
                    Id = demande.Id,
                    NomCitoyen = demande.NomCitoyen,
                    Statut = demande.Statut,
                    TypeDemande = demande.TypeDemande

                };


                return Ok(demandeReadDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la demande {id}");
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpGet("statut/{statut}")]

        public async Task<ActionResult<IEnumerable<DemandeReadDTO>>> GetByStatut(StatutEnum statut)
        {
            try
            {
                var demandes = await _repository.GetByStatusAsync(statut);
                IEnumerable<DemandeReadDTO> demandeReadDTOs = demandes.Select(
                                        d => new DemandeReadDTO()
                                        {
                                            Id = d.Id,
                                            NomCitoyen = d.NomCitoyen,
                                            Statut = d.Statut,
                                            TypeDemande = d.TypeDemande

                                        }
                    );
                return Ok(demandeReadDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes par statut {Statut}", statut);
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DemandeReadDTO>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
            {
                return BadRequest("Le terme de recherche doit contenir au moins 3 caractères");
            }

            try
            {
                var demandes = await _repository.SearchAsync(term);
                IEnumerable<DemandeReadDTO> demandeReadDTOs = demandes.Select(
                                       d => new DemandeReadDTO()
                                       {
                                           Id = d.Id,
                                           NomCitoyen = d.NomCitoyen,
                                           Statut = d.Statut,
                                           TypeDemande = d.TypeDemande

                                       }
                   );
                return Ok(demandeReadDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche de demandes");
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AgentPolicy")]
        public async Task<ActionResult<DemandeReadDTO>> Create([FromBody] CreateDemandeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Demande demande = new Demande
                {
                    NomCitoyen = dto.NomCitoyen.Trim(),
                    TypeDemande = dto.TypeDeDemande.Trim(),
                    Statut = StatutEnum.EnCours,
                    DateCreation = DateTime.Now
                };

                int id = await _repository.CreateAsync(demande);
                

                return CreatedAtAction(nameof(GetById), new { id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la demande");
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDemandeDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le corps");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingDemande = await _repository.GetByIdAsync(id);
                if (existingDemande == null)
                {
                    return NotFound($"Demande avec l'ID {id} introuvable");
                }

                //Demande demande = new Demande
                //{
                //    Id = dto.Id,
                //    NomCitoyen = dto.NomCitoyen.Trim(),
                //    TypeDemande = dto.TypeDeDemande.Trim(),
                //    Statut = dto.Statut,
                //    //DateCreation = existingDemande.DateCreation
                //};

                existingDemande.Statut = dto.Statut;
                existingDemande.TypeDemande = dto.TypeDeDemande;
                existingDemande.NomCitoyen = dto.NomCitoyen;

                var success = await _repository.UpdateAsync(existingDemande);
                if (!success)
                {
                    return StatusCode(500, "Échec de la mise à jour");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la demande {Id}", id);
                return StatusCode(500, "Une erreur est survenue");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID invalide");
            }

            try
            {
                Demande? existingDemande = await _repository.GetByIdAsync(id);
                if (existingDemande == null)
                {
                    return NotFound($"Demande avec l'ID {id} introuvable");
                }

                bool success = await _repository.DeleteAsync(id);
                if (!success)
                {
                    return StatusCode(500, "Échec de la suppression");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la demande {Id}", id);
                return StatusCode(500, "Une erreur est survenue");
            }
        }
    }
}
