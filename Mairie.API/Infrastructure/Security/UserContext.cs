using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Security.Principal;

namespace Mairie.API.Infrastructure.Security
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger _logger;

        public UserContext(IHttpContextAccessor accessor, ILogger<UserContext> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

#pragma warning disable CA1416 // Validate platform compatibility
        public string WindowsId
        {
            get {
                    if (_accessor.HttpContext?.User.Identity is WindowsIdentity windowsIdentity)
                    {
                        var info = windowsIdentity?.User?.AccountDomainSid.Value;
                    if (info != null) return info;
                        else throw new IdentityNotMappedException();
                }
                    else
                    {
                        throw new IdentityNotMappedException();
                    }
            }
        }
        public string? DisplayName
        {
            get 
            { 
                  ClaimsPrincipal? user = _accessor.HttpContext?.User;

                  if(user?.Identity is WindowsIdentity windowsIdentity)
                    {
                    try
                    {
                        using var ctx = new PrincipalContext(ContextType.Domain);
                        using var usr = UserPrincipal.FindByIdentity(ctx, windowsIdentity.Name);
                        if (usr != null)
                        {
                            string? dName = usr.DisplayName;
                            _logger.LogInformation("WindowIdentity trouvé : {dName}", dName);
                            return dName;
                        }
                        else
                        {
                            _logger.LogWarning("Utilisateur AD non trouvé : {Name}", windowsIdentity.Name);
                            return windowsIdentity.Name;
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        _logger.LogWarning("Impossible de se connecter à l'AD: {error}", error);
                        var info = windowsIdentity?.Name;
                        if (info != null) return info;
                        else throw new IdentityNotMappedException();
                    }
                  
                    }

                string nonWindowsName = user?.Identity?.Name ?? "Nom non récupéré";
                _logger.LogWarning("WindowsIdentity non trouvé, MAIS utilisateur suivant récupéré : {nonWindowsName}", nonWindowsName);
                throw new IdentityNotMappedException("Pas d'utilisateur windows trouvé");
            }
        }

        public string AccountName
        {
            get
            {
                if (_accessor.HttpContext?.User.Identity is WindowsIdentity windowsIdentity)
                {
                    var info = windowsIdentity.Name;
                    if (info != null) return info;
                    else throw new IdentityNotMappedException();
                }
                else
                {
                    throw new IdentityNotMappedException();
                }
            }
        }

         
        public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
#pragma warning restore CA1416 // Validate platform compatibility

    }
}
