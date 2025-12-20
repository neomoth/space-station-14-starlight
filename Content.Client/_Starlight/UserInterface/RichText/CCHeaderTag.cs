using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class CCHeaderTag : IMarkupTagHandler
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
    [Dependency] private readonly IResourceCache _resourceCache = default!;

    private static readonly List<string> _headers = new()
    {
        "cc",
        "cad",
        "ced",
        "crd",
        "cmd",
        "csd",
        "ccd",
        "cdd",
        "cid",
        "csod"
    };
    
    public string Name => "CCHeader";

    public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        if (node.Value.TryGetString(out var value))
            if (!_headers.Contains(value))
                value = "cc";

        value ??= "cc";
        var icon = new TextureRect
        {
            Texture = _resourceCache.GetTexture($"/Textures/_Starlight/Logo/header/{value}.png"),
            TextureScale = new Vector2(1, 1),
            SetHeight = 0,
            AlwaysRender = false,
            
        };

        foreach (var attribute in node.Attributes.Where(attribute => attribute.Key == "margin"))
        {
            if (!attribute.Value.TryGetLong(out var useMargin)) break;
            if (useMargin != 1) break;
            icon.Margin = new Thickness(0, 15, 0, 0);
        }

        control = icon;
        return true;
    }
}