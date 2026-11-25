using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Ghost;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Commands.Entities.World;

namespace Content.Server._Starlight.Administration;

public sealed partial class ToolshedTeleportPermissionSystem : EntitySystem
{
    [Dependency] private IAdminManager _admin = null!;

    [SubscribeLocalEvent]
    private void OnTeleportAttempt(ref ToolshedTeleportAttemptEvent ev)
    {
        if (ev.User is not { } user)
            return;

        if (_admin.HasAdminFlag(user, AdminFlags.Debug))
            return;

        // Non-admins can only teleport to other players, and can't teleport other players/entities to anything.

        if (user.AttachedEntity != ev.Teleporter)
        {
            ev.Cancelled = true;
            ev.Reason = "You can't teleport other entities.";
            return;
        }

        if (HasComp<GhostComponent>(ev.Teleporter) && HasComp<ActorComponent>(ev.Target))
            return;

        ev.Cancelled = true;
        ev.Reason = "You can only teleport while adminned or ghosted.";
    }
}
