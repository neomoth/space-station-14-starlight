using Robust.Shared.Console;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Server._Starlight.Warps;

public sealed class WarpNameCompletionParser : CustomCompletionParser<string>
{
    public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
    {
        var warps = WarpCommand.GetWarps();
        List<CompletionOption> opts = [];
        foreach (var (warpName, gridName, _) in warps)
            opts.Add(new CompletionOption($"\"{warpName}\"", gridName,
                CompletionOptionFlags.NoEscape | CompletionOptionFlags.NoQuote));
        return CompletionResult.FromHintOptions(opts, GetArgHint(arg));
    }
}
