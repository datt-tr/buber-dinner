using Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Authentication.Queries.Login;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Services.Authentication.Common;
using BuberDinner.Domain.Common.Errors;
using BuberDinner.Domain.Entities;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
  private readonly IJwtTokenGenerator _jwtTokenGenerator;
  private readonly IUserRepository _userRepository;

  public LoginCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
  {
    _jwtTokenGenerator = jwtTokenGenerator;
    _userRepository = userRepository;
  }

  public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
  {
    if (_userRepository.GetUserByEmail(query.Email) is not User user)
    {
      return Errors.Authentication.InvalidCredentials;
    }

    // Validate the Password is correct
    if (user.Password != query.Password)
    {
      return new[] { Errors.Authentication.InvalidCredentials };
    }

    // Create JWT token
    var token = _jwtTokenGenerator.GenerateToken(user);

    return new AuthenticationResult(
      user,
      token);
  
  }
}