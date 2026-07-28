using System.Runtime.InteropServices;
using Content.Server._Starlight.Toolshed;
using Content.Server.Administration;
using Content.Server.Voting;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Content.Shared.Voting;
using Robust.Shared.Player;
using Robust.Shared.Toolshed;

namespace Content.Server._Starlight.Voting;

[ToolshedCommand]
[AdminCommand(AdminFlags.Round)]
public sealed partial class VoteCommand : ToolshedCommand
{
    [Dependency] private IVoteManager _vote = default!;

    [CommandImplementation("cancel")]
    public void CancelVote(IInvocationContext ctx, VoteId id)
    {
        if (!_vote.TryGetVote(id.Id, out var vote))
        {
            ctx.WriteMarkup($"[color=red]No vote with id {id} exists.[/color]");
            return;
        }
        vote.Cancel();
        ctx.WriteLine($"Cancelled vote with id {id}.");
    }

    [CommandImplementation("cancelall")]
    public void CancelAll(IInvocationContext ctx)
    {
        foreach(var vote in _vote.ActiveVotes)
            vote.Cancel();
        ctx.WriteLine($"Cancelled all active votes.");
    }

    [CommandImplementation("map")]
    public void CreateMapVote(IInvocationContext ctx) =>
        _vote.CreateStandardVote(ctx.Session, StandardVoteType.Map);

    [CommandImplementation("preset")]
    public void CreatePresetVote(IInvocationContext ctx) =>
        _vote.CreateStandardVote(ctx.Session, StandardVoteType.Preset);

    [CommandImplementation("restart")]
    public void CreateRestartVote(IInvocationContext ctx) =>
        _vote.CreateStandardVote(ctx.Session, StandardVoteType.Preset);

    [CommandImplementation("kick")]
    public void CreateKickVote(IInvocationContext ctx, ICommonSession target, string reason) =>
        _vote.CreateStandardVote(ctx.Session, StandardVoteType.Votekick, [target.Name, reason]);

    [CommandImplementation("custom")]
    public void CreateCustomVote(IInvocationContext ctx, string title, bool showVotes, ValueArray<string> options)
    {
        if (options.Invalid) return;

        if (options.Array.Length > 9)
        {
            ctx.WriteMarkup("[color=yellow]Votes can only have up to 9 options![/color]");
            return;
        }

        _vote.CreateVote(new VoteOptions
        {
            Title = title,
            InitiatorPlayer = ctx.Session,
            DisplayVotes = showVotes,
        });
    }
}
