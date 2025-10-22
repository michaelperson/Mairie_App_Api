using Microsoft.AspNetCore.Authorization;

namespace Mairie.API.Infrastructure.Security
{
    public class DemandeOwnerRequirement: IAuthorizationRequirement
    {

        public string Action { get; }

        public DemandeOwnerRequirement(string action)
        {
            Action=action;
        }
    }
}
