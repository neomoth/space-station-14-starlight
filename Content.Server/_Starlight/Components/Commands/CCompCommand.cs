using Content.Server.Administration;
using Content.Shared._Starlight.Components;
using Content.Shared.Administration;
using Robust.Shared.Player;
using Robust.Shared.Toolshed;

namespace Content.Server._Starlight.Components.Commands;

/// <summary>
/// Command for handling management of clientside components. CANNOT return entityuid for a pipe chain as returning Task&lt;EntityUid&gt; doesn't seem to fucking work lmao
/// </summary>
[ToolshedCommand, AdminCommand(AdminFlags.Fun)]
public sealed class CCompCommand : ToolshedCommand
{
    [Dependency] private readonly IEntitySystemManager _entSysMan = default!;
    [Dependency] protected readonly ILogManager LogManager = default!;
    
    private ISawmill? log;
    
    [CommandImplementation("ensure")]
    public async void Ensure([PipedArgument] EntityUid targetEntity, string compName)
    {
        try
        {
            log ??= LogManager.GetSawmill("ccomp");
            var ccontrol = _entSysMan.GetEntitySystem<ClientComponentControlSystem>();
            if (!EntityManager.TryGetNetEntity(targetEntity, out var netEntity)) return;
            var ev = new CreateClientComponentEvent { NetEntityUid = netEntity.Value, ComponentName = compName, };
            var results = await ccontrol.SendToAllClients(ev);
            foreach (var result in results)
            {
                log.Log(LogLevel.Debug, $"result from {result.Key}: {result.Value!.ControlType}, {result.Value!.ControlSuccess}, {result.Value?.Message}");
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
    
    [CommandImplementation("write")]
    public async void Write([PipedArgument] EntityUid targetEntity, string compName, string path, string data)
    {
        try
        {
            log ??= LogManager.GetSawmill("ccomp");
            var ccontrol = _entSysMan.GetEntitySystem<ClientComponentControlSystem>();
            if (!EntityManager.TryGetNetEntity(targetEntity, out var netEntity)) return;
            var ev = new WriteClientComponentEvent
            {
                NetEntityUid = netEntity.Value, ComponentName = compName, ValuePath = path, NewValue = data,
            };
            var results = await ccontrol.SendToAllClients(ev);
            foreach (var result in results)
            {
                log.Log(LogLevel.Debug, $"result from {result.Key}: {result.Value!.ControlType}, {result.Value!.ControlSuccess}, {result.Value?.Message}");
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
}