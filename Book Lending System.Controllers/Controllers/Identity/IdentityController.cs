using Book_Lending_System.APIs.Controllers;
using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Application.Features.Identity.Commands.Models;
using Book_Lending_System.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Book_Lending_System.Controllers.Controllers.Identity
{
    public class IdentityController:BaseController
    {
        private readonly IMediator mediator;

        public IdentityController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await mediator.Send(new RegisterCommand() { RegisterDTO = registerDTO });
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await mediator.Send(new LoginCommand() { LoginDTO = loginDTO });
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDTO emailDTO)
        {
            var result = await mediator.Send(new ConfirmUserEmail() { EmailDTO = emailDTO });
            return StatusCode((int)result.StatusCode, result);
        }


    }
}
