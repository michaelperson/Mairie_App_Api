using Mairie.Domain.DTOs;
using Mairie.Domain.Entities;
using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Mairie.API.Infrastructure.Security
{
    public class DemandeRequirementHandler : AuthorizationHandler<DemandeOwnerRequirement, Demande>
    {
        private readonly IUserContext _userContext;
        private readonly IUserRoleRepository _repo;

        public DemandeRequirementHandler(IUserContext userContext, IUserRoleRepository userRoleRepository)
        {
            _userContext = userContext;
            _repo = userRoleRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DemandeOwnerRequirement requirement, Demande resource)
        {
            if (!_userContext.IsAuthenticated){
                context.Fail();
                return;
            }

            IEnumerable<string> roles = await _repo.GetRolesByWindowsIdAsync(_userContext.WindowsId); 



            ///faire les test business
            ///si le test est ok ==> context.Succeed();
            ///sinon context.Fail();
            ///
            switch (requirement.Action)
            {
                case "Create": context.Succeed(requirement); break;
                case "Update":
                    if (roles.Contains("ChefService")) context.Succeed(requirement);
                    else
                    {
                        if(resource.Statut!= Domain.Enumerations.StatutEnum.Annulee
                            && resource.Statut!= Domain.Enumerations.StatutEnum.EnCours
                            )
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                    }
                        break;
                case "Delete": context.Succeed(requirement); break;
                case "Read": context.Succeed(requirement); break;
                default:
                    context.Fail();
                    break;
            }
        }
    }
}
