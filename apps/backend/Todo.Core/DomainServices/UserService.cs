using Todo.Core.Entities;
using Todo.Core.Exceptions;
using Todo.Core.Policies;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public class UserService(
    IUserRepository userRepository,
    IUserUpdatePolicy userUpdatePolicy)
    : IUserService
{
    public Task ChangeFullNameAsync(User requestedBy, User userToUpdate, string newFullName)
    {
        EnsureCanUpdate(requestedBy, userToUpdate);
        userToUpdate.ChangeFullName(newFullName);

        return Task.CompletedTask;
    }

    public async Task<Email> PrepareEmailChangeAsync(User requestedBy, User userToUpdate, string newEmail)
    {
        EnsureCanUpdate(requestedBy, userToUpdate);

        if (userToUpdate.Email == newEmail)
        {
            throw new EmailUnchangedException();
        }

        var email = new Email(newEmail);
        var existingUser = await userRepository.GetUserByEmailAsync(email);

        if (existingUser is not null && existingUser.UserId != userToUpdate.UserId)
        {
            throw new EmailAlreadyInUseException(email);
        }

        return email;
    }

    public async Task ChangeUsernameAsync(User requestedBy, User userToUpdate, string newUsername)
    {
        EnsureCanUpdate(requestedBy, userToUpdate);

        var username = new Username(newUsername);
        var existingUser = await userRepository.GetUserByUsernameAsync(username);

        if (existingUser is not null && existingUser.UserId != userToUpdate.UserId)
        {
            throw new UsernameAlreadyInUseException(username);
        }

        userToUpdate.ChangeUsername(username);
    }

    public Task ChangePasswordAsync(User requestedBy, User userToUpdate, string securedPassword)
    {
        EnsureCanUpdate(requestedBy, userToUpdate);
        userToUpdate.ChangePassword(securedPassword);

        return Task.CompletedTask;
    }

    public Task ChangeProfileVisibilityAsync(User requestedBy, User userToUpdate, bool isProfileVisible)
    {
        EnsureCanUpdate(requestedBy, userToUpdate);
        userToUpdate.ChangeProfileVisibility(isProfileVisible);
        
        return Task.CompletedTask;
    }

    private void EnsureCanUpdate(User requestedBy, User userToUpdate)
    {
        if (!userUpdatePolicy.CanUpdate(userToUpdate, requestedBy))
        {
            throw new UserAccessDeniedException();
        }
    }
}
