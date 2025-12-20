using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class HRTag : IMarkupTagHandler
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystem = default!;
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    
    private static readonly Dictionary<string, Color> _colorLookupTable = new()
    {
        ["cc"] = Color.FromHex("#006600"),
        ["cad"] = Color.FromHex("#1155CC"),
        ["ccd"] = Color.FromHex("#B45F06"),
        ["ced"] = Color.FromHex("#FFA238"),
        ["cid"] = Color.FromHex("#660089"),
        ["cmd"] = Color.FromHex("#0AAFC3"),
        ["crd"] = Color.FromHex("#9900FF"),
        ["cdd"] = Color.FromHex("#6AA84F"),
        ["csod"] = Color.FromHex("#741B47"),
        ["csd"] = Color.FromHex("#CC0000"),
    };
    
    public string Name => "hr";

    public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        if (!node.Value.TryGetColor(out var value))
        {
            if (!node.Value.TryGetString(out var str)) value = Color.Black;
            else value = _colorLookupTable.TryGetValue(str, out var color) ? color : Color.Black;
        }
        var icon = new TextureRect
        {
            Texture = _resourceCache.GetTexture("/Textures/_Starlight/Logo/hr.png"),
            TextureScale = new Vector2(1, 1),
            Modulate = value.Value,
            SetHeight = 10,
            SetWidth = 0,
        };

        control = icon;
        return true;
    }
}