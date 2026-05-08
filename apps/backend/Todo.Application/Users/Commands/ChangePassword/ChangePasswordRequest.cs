namespace Todo.Application.Users.Commands.ChangePassword;

public record ChangePasswordRequest(string OldPassword, string NewPassword);
