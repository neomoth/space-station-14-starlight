using Robust.Shared.Timing;

namespace Content.Shared._Starlight.SpawnPad;

public abstract class SharedSpawnPadSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
}