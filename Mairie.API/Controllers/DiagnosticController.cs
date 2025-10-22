using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mairie.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly IUserContext _userContext;

        public DiagnosticController(IUserContext userContext)
        {
            _userContext = userContext;
        }

        [HttpGet("/health")]
        public IActionResult Get()
        {
            return Ok("Api Mairie is running");
        }

        [HttpGet("/secure-information")]
        [Authorize(Policy = "AdministrateurPolicy")]
        public IActionResult GetSecureInformation()
        {
            string Message = @$"Informations Confidentielles : 
                                 SSid : {_userContext.WindowsId}
                                 DisplayName : {_userContext.DisplayName}
                                 AccountName: {_userContext.AccountName}";

            return Ok(Message);
        }
    }
}
