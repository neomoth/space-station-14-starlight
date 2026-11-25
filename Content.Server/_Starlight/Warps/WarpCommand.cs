using System.Linq;
using Content.Server.Administration.Managers;
using Content.Shared._Starlight.Commands;
using Content.Shared.Administration;
using Content.Shared.Ghost;
using Content.Shared.Warps;
using Robust.Server.GameObjects;
using Robust.Shared.Toolshed;

namespace Content.Server._Starlight.Warps;

[AnyCommand]
[ToolshedCommand]
public sealed partial class WarpCommand : ToolshedCommand
{
    [Dependency] private IAdminManager _admin = null!;
    private TransformSystem? _xform;

    [CommandImplementation]
    public void Warp(IInvocationContext ctx, [CommandArgument(typeof(WarpNameCompletionParser))] string warpName)
    {
        if (SharedCommandHelpers.NoSession(ctx)) return;
        if (ctx.Session?.AttachedEntity is null)
        {
            CommandMarkup.Error(ctx, "No entity is attached to this session.");
            return;
        }

        var executor = ctx.Session.AttachedEntity.Value;

        // Can only use warp if you are adminned and have +Admin, or if you are a ghosted player.
        if (!HasComp<GhostComponent>(executor) &&
            !_admin.HasAdminFlag(ctx.Session, AdminFlags.Admin))
        {
            CommandMarkup.Error(ctx, "You can only use this command if you are ghosted or adminned.");
            return;
        }

        var warps = GetWarps().Select(x => (x.warpName, x.uid)).ToDictionary();
        if (!warps.TryGetValue(warpName, out var warp))
        {
            CommandMarkup.Error(ctx, "Specified warp does not exist.");
            return;
        }

        _xform ??= GetSys<TransformSystem>();
        _xform.SetCoordinates(executor, Transform(warp).Coordinates);
        _xform.AttachToGridOrMap(executor);

        ctx.WriteLine($"Warped to {warpName}.");
    }

    public static List<(string warpName, string gridName, EntityUid uid)> GetWarps()
    {
        var entMan = IoCManager.Resolve<IEntityManager>();

        List<(string warpName, string gridName, EntityUid uid)> warps = [];
        var query = entMan.EntityQueryEnumerator<WarpPointComponent, MetaDataComponent>();
        while (query.MoveNext(out var uid, out var warp, out var meta))
            warps.Add((warp.Location ?? meta.EntityName,
                entMan.GetComponent<MetaDataComponent>(entMan.GetComponent<TransformComponent>(uid).ParentUid)
                    .EntityName, uid));
        return warps;
    }
}
