using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public interface IUserService
{
    Task ChangeFullNameAsync(User requestedBy, User userToUpdate, string newFullName);
    Task<Email> PrepareEmailChangeAsync(User requestedBy, User userToUpdate, string newEmail);
    Task ChangeUsernameAsync(User requestedBy, User userToUpdate, string newUsername);
    Task ChangePasswordAsync(User requestedBy, User userToUpdate, string securedPassword);
    Task ChangeProfileVisibilityAsync(User requestedBy, User userToUpdate,  bool isProfileVisible);
}
