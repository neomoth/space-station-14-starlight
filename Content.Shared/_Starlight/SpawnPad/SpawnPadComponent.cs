using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.SpawnPad;

[RegisterComponent]
public sealed partial class SpawnPadComponent : Component
{
    /// <summary>
    /// The entity prototype to spawn
    /// </summary>
    [DataField] public EntProtoId Prototype;
    /// <summary>
    /// One-off spawn signal. Device must be on to work.
    /// </summary>
    [DataField] public string SpawnOnceSignal = "SpawnOnce";
    /// <summary>
    /// Powers on the device
    /// </summary>
    [DataField] public string PowerOnSignal = "PowerOn";
    /// <summary>
    /// Powers off the device
    /// </summary>
    [DataField] public string PowerOffSignal = "PowerOff";
    /// <summary>
    /// Toggles power
    /// </summary>
    [DataField] public string PowerToggleSignal = "PowerToggle";
    /// <summary>
    /// Activate automatic respawning on entity death
    /// </summary>
    [DataField] public string ActivateRespawns = "ActivateRespawns";
    /// <summary>
    /// Deactivate automatic respawning on entity death
    /// </summary>
    [DataField] public string DeactivateRespawns = "DeactivateRespawns";
    /// <summary>
    /// Toggles automatic respawning on entity death
    /// </summary>
    [DataField] public string ToggleRespawns = "ToggleRespawns";
    /// <summary>
    /// Delay between entity dying and it being respawned by the pad
    /// </summary>
    [DataField] public float RespawnDelay;
    /// <summary>
    /// Whether the entity needs to die before another is allowed to be spawned or not
    /// </summary>
    [DataField] public bool AllowRespawnWhenAlive = true;
    /// <summary>
    /// Whether the device is on or not
    /// </summary>
    [ViewVariables] public bool Enabled;
    /// <summary>
    /// Whether to automatically respawn the entity on death
    /// </summary>
    [ViewVariables] public bool DoRespawns;
    /// <summary>
    /// The entity that was spawned and is being tracked by the pad
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)] public EntityUid TrackedEntity;
    /// <summary>
    /// When to spawn the next entity
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)] public TimeSpan NextSpawnTime;
}