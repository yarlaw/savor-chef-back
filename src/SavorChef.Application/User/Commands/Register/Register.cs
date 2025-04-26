using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;
using SavorChef.Application.Models;
using SavorChef.Domain.Entities;
using SavorChef.Domain.Enums;

namespace SavorChef.Application.User.Commands.Register
{
    public class RegisterCommand : IRequest<JwtTokenResult>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

   
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, JwtTokenResult>
    {
        private readonly IIdentityService _identityService;

        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<JwtTokenResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var (result, tokens) = await _identityService.SignUpAsync(request.Username, request.Email, request.Password);

            if (!result.Succeeded)
            {
                throw new ApplicationException("Registration failed." + string.Join("; ", result.Errors));
            }

            return tokens!;
        }
    }

}