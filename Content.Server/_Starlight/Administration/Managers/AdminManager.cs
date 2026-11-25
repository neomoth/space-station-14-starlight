using Content.Shared.Administration;
using Robust.Shared.Toolshed;

// ReSharper disable CheckNamespace
namespace Content.Server.Administration.Managers;

public sealed partial class AdminManager
{
    private bool TryGetSubCommandFlags(CommandSpec command, out AdminFlags[]? flags)
    {
        flags = null;
        if (command.SubCommand is null)
            return false;

        var fullName = $"{command.Cmd.Name}:{command.SubCommand}";

        return _toolshedCommandPermissions.AnyCommands.Contains(fullName) ||
            _toolshedCommandPermissions.AdminCommands.TryGetValue(fullName, out flags);
    }
}
