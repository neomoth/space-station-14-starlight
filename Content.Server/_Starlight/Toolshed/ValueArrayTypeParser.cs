using System.Diagnostics.CodeAnalysis;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Content.Server._Starlight.Toolshed;

public sealed class ValueArrayTypeParser<T> : TypeParser<ValueArray<T>>
{
    public override bool TryParse(ParserContext ctx, out ValueArray<T> result)
    {
        ctx.ConsumeWhitespace();

        if (!ctx.EatMatch('['))
        {
            ctx.Error = new ExpectedTokenError(["["]);
            result = new ValueArray<T>([], true);
            return false;
        }

        var values = new List<T>();

        while (true)
        {
            ctx.ConsumeWhitespace();

            // Empty array or end of array.
            if (ctx.EatMatch(']'))
            {
                result = new ValueArray<T>(values.ToArray());
                return true;
            }

            if (!Toolshed.TryParse(ctx, out T? value))
            {
                result = new ValueArray<T>([], true);
                return false;
            }

            values.Add(value);

            ctx.ConsumeWhitespace();

            if (ctx.EatMatch(','))
                continue;

            if (ctx.EatMatch(']'))
            {
                result = new ValueArray<T>(values.ToArray());
                return true;
            }

            ctx.Error = new ExpectedTokenError([",", "]"]);
            result = new ValueArray<T>([], true);
            return false;
        }
    }

    public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
{
    var hint = ToolshedCommand.GetArgHint(arg, typeof(T));

    ctx.ConsumeWhitespace();

    if (!ctx.EatMatch('['))
    {
        return CompletionResult.FromHintOptions(
            new[]
            {
                new CompletionOption(
                    "[",
                    Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape)
            },
            hint);
    }

    while (true)
    {
        ctx.ConsumeWhitespace();

        var restore = ctx.Save();

        if (!Toolshed.TryParse(ctx, out T? _))
        {
            ctx.Restore(restore);
            Log.Info($"atuocomplete: {ctx.Input}");
            return Toolshed.TryAutocomplete(ctx, typeof(T), arg);
        }

        ctx.ConsumeWhitespace();

        // Value parsed successfully. We are now waiting on ',' or ']'.
        if (ctx.PeekRune() is null)
        {
            return CompletionResult.FromHintOptions(
                new[]
                {
                    new CompletionOption(
                        ", ",
                        Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape),
                    new CompletionOption(
                        "]",
                        Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape)
                },
                hint);
        }

        if (ctx.EatMatch(','))
            continue;

        if (ctx.EatMatch(']'))
            return CompletionResult.FromHint(hint);

        return CompletionResult.FromHintOptions(
            new[]
            {
                new CompletionOption(
                    ", ",
                    Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape),
                new CompletionOption(
                    "]",
                    Flags: CompletionOptionFlags.PartialCompletion | CompletionOptionFlags.NoEscape)
            },
            hint);
    }
}
}

public sealed class ExpectedTokenError(string[] expectedTokens) : ConError
{
    public override FormattedMessage DescribeInner() =>
        FormattedMessage.FromUnformatted($"Expected one of the following tokens: {string.Join(", ", expectedTokens)}");
}

public readonly record struct ValueArray<T>(T[] Array, bool Invalid = false);
