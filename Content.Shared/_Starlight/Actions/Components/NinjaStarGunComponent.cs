using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Actions.Components;

[RegisterComponent]
public sealed partial class NinjaStarGunComponent : Component
{
    [DataField] public EntProtoId Action = "NinjaStarGun";
    [DataField] public EntProtoId GunProto = "WeaponNinjaStarGun";
    [ViewVariables] public EntityUid? ActionEntity;
    [ViewVariables] public EntityUid? Gun;
}