using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Mairie.API.Infrastructure.Security
{
    /// <summary>
    /// Transformer les rôles DB en Claims pour pouvoir appliquer les Policy définies
    /// </summary>
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserContext _userContext;
        private readonly IUserRoleRepository _roleRepository;

        public RoleClaimsTransformation(IUserContext userContext, IUserRoleRepository roleRepository)
        {
            _userContext=userContext;
            _roleRepository=roleRepository;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //Si pas authentifié ==> je retourne le principal sans modification
            if (principal.Identity is not { IsAuthenticated: true }) return principal;

            //Je récupère les rôles dans la base de données liés à mon utilisateur
            IEnumerable<string> roles = await _roleRepository.GetRolesByWindowsIdAsync(_userContext.WindowsId);
            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;
            //J'ajoute les rôles en tant que claims
            foreach (string role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            //retourne mon principal completé par mes rôles
            return principal;
        }
    }
}
