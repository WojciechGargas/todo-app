namespace Todo.Application.Commands.ProfileCommands;

public record ChangePasswordRequest(string OldPassword, string NewPassword);