using Content.Server.Actions;
using Content.Server.Hands.Systems;
using Content.Server.Item;
using Content.Shared._Starlight.Actions.Components;
using Content.Shared._Starlight.Actions.EntitySystems;
using Content.Shared._Starlight.Actions.Events;

namespace Content.Server._Starlight.Actions.EntitySystems;

public sealed class NinjaStarGunSystem : SharedNinjaStarGunSystem
{
    [Dependency] private readonly ItemSystem _item = default!;
    [Dependency] private readonly HandsSystem _hands = default!;
    [Dependency] private readonly ActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NinjaStarGunComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<NinjaStarGunComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<NinjaStarGunActionEvent>(OnAction);
    }

    private void OnStartup(EntityUid uid, NinjaStarGunComponent comp, ComponentStartup ev) =>
        _action.AddAction(uid, ref comp.ActionEntity, comp.Action);

    private void OnShutdown(EntityUid uid, NinjaStarGunComponent comp, ComponentShutdown ev) =>
        _action.RemoveAction(uid, comp.ActionEntity);

    private void OnAction(NinjaStarGunActionEvent ev)
    {
        if (ev.Handled) return;
        var uid = ev.Performer;
        if (!TryComp<NinjaStarGunComponent>(uid, out var comp)) return;

        if (comp.Gun is null)
            foreach (var hand in _hands.EnumerateHands(uid))
            {
                if (!_hands.HandIsEmpty(uid, hand)) continue;
                var ent = Spawn(comp.GunProto);
                _hands.DoPickup(uid, hand, ent);
                comp.Gun = ent;
                break;
            }
        else
        {
            QueueDel(comp.Gun);
            comp.Gun = null;
        }
        
        ev.Handled = true;
    }
}