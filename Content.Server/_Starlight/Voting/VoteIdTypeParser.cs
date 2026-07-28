using System.Linq;
using Content.Server.Voting.Managers;
using Robust.Shared.Console;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Toolshed.TypeParsers.Math;

namespace Content.Server._Starlight.Voting;

public sealed partial class VoteIdTypeParser : TypeParser<VoteId>
{
    [Dependency] private IVoteManager _vote = default!;

    public override bool TryParse(ParserContext ctx, out VoteId result)
    {
        result = default;
        if (!Toolshed.TryParse(ctx, out int? num))
        {
            ctx.Error = new ExpectedNumericError();
            return false;
        }

        result = new VoteId(num.Value);
        return true;
    }

    public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
    {
        var options = _vote.ActiveVotes
            .OrderBy(v => v.Id)
            .Select(v => new CompletionOption(v.Id.ToString(), $"{v.Title} ({v.InitiatorText})")).ToList();
        return options.Count == 0
            ? new CompletionResult([], "[There are no active votes to cancel.]")
            : CompletionResult.FromHintOptions(options, ToolshedCommand.GetArgHint(arg, typeof(VoteId)));
    }
}

public readonly record struct VoteId(int Id);
