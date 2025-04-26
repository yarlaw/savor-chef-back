using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;

namespace SavorChef.Application.User.Commands.Login
{
    public class LoginCommand : IRequest<JwtTokenResult>
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

   
    public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtTokenResult>
    {
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<JwtTokenResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var (result, tokens) = await _identityService.LoginAsync(request.Identifier, request.Password);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid identifier or password.");
            }

            return tokens!;
        }
    }

}