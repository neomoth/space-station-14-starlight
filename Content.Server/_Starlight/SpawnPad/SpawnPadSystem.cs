using Content.Server.Chat.Systems;
using Content.Server.DeviceLinking.Systems;
using Content.Server.Popups;
using Content.Shared._Starlight.SpawnPad;
using Content.Shared.Audio;
using Content.Shared.DeviceLinking;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.DeviceNetwork;
using Robust.Server.GameObjects;
using Robust.Server.GameStates;
using Robust.Shared.Timing;

namespace Content.Server._Starlight.SpawnPad;

public sealed class SpawnPadSystem : SharedSpawnPadSystem
{
    [Dependency] private readonly TransformSystem _xformSystem = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly SharedPointLightSystem _pointLightSystem = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSoundSystem = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly PvsOverrideSystem _pvs = default!;
    [Dependency] private readonly DeviceLinkSystem _signalSystem = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpawnPadComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SpawnPadComponent, SignalReceivedEvent>(OnSignalReceived);
    }
    
    private void OnInit(EntityUid uid, SpawnPadComponent component, ComponentInit args) =>
        _signalSystem.EnsureSinkPorts(uid, component.PowerOnSignal, component.PowerOffSignal, component.SpawnOnceSignal,
            component.ActivateRespawns, component.DeactivateRespawns);

    private void OnSignalReceived(EntityUid uid, SpawnPadComponent component, SignalReceivedEvent args)
    {
        var state = SignalState.Momentary;
        args.Data?.TryGetValue(DeviceNetworkConstants.LogicState, out state);

        if (args.Port == component.PowerOnSignal)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.Enabled = true;
            }
        }
        else if (args.Port == component.PowerOffSignal)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.Enabled = false;
            }
        }
        else if (args.Port == component.PowerToggleSignal)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.Enabled = !component.Enabled;
            }
        }
        else if (args.Port == component.ActivateRespawns)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.DoRespawns = true;
            }
        }
        else if (args.Port == component.DeactivateRespawns)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.DoRespawns = false;
            }
        }
        else if (args.Port == component.ToggleRespawns)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                component.DoRespawns = !component.DoRespawns;
            }
        }
        else if (args.Port == component.SpawnOnceSignal)
        {
            if (state is SignalState.High or SignalState.Low)
            {
                DoSpawn(uid, component);
            }
        }
    }

    public override void Update(float frameTime)
    {
        
    }

    private void DoSpawn(EntityUid uid, SpawnPadComponent component)
    {
        QueueDel(component.TrackedEntity);
        component.TrackedEntity = Spawn(component.Prototype, Transform(uid).Coordinates);
    }
}