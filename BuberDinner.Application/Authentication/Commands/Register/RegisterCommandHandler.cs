using Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Services.Authentication.Common;
using BuberDinner.Domain.Common.Errors;
using BuberDinner.Domain.Entities;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
  private readonly IJwtTokenGenerator _jwtTokenGenerator;
  private readonly IUserRepository _userRepository;

  public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
  {
    _jwtTokenGenerator = jwtTokenGenerator;
    _userRepository = userRepository;
  }

  public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
  {
    // Check if user already exists (Validate the User doesnt exist)
    if (_userRepository.GetUserByEmail(command.Email) is not null)
    {
      return Errors.User.DuplicateEmail;
    }

    // Create user (generate unique ID) & Persist to DB 
    var user = new User
    {
      FirstName = command.FirstName,
      LastName = command.LastName,
      Email = command.Email,
      Password = command.Password
    };

    _userRepository.Add(user);

    // Create JWT Token
    var token = _jwtTokenGenerator.GenerateToken(user);

    return new AuthenticationResult(
      user,
      token);
  }
}