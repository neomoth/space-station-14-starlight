using Content.Shared.DisplacementMap;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory;

[RegisterComponent, NetworkedComponent]
[Access(typeof(InventorySystem), typeof(InventorySystem.InventorySlotEnumerator))] // Starlight-edit
[AutoGenerateComponentState(true)]
public sealed partial class InventoryComponent : Component
{
    /// <summary>
    /// The template defining how the inventory layout will look like.
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables] // use the API method
    public ProtoId<InventoryTemplatePrototype> TemplateId = "human";

    /// <summary>
    /// For setting the TemplateId.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<InventoryTemplatePrototype> TemplateIdVV
    {
        get => TemplateId;
        set => IoCManager.Resolve<IEntityManager>().System<InventorySystem>().SetTemplateId((Owner, this), value);
    }
    
    // Starlight begin
    /// <summary>
    /// <see cref="Shared.Inventory.SlotDefinition"/>s written into here via VV should automatically propagate.
    /// </summary>
    [DataField, AutoNetworkedField] public List<SlotDefinition>? CustomSlots = [];
    
    /// <summary>
    /// <see cref="Robust.Shared.Containers.ContainerSlot"/>s managed by <see cref="CustomSlots"/> 
    /// </summary>
    [ViewVariables] public List<ContainerSlot>? CustomContainers = [];
    // Starlight end

    [DataField, AutoNetworkedField]
    public string? SpeciesId;


    [ViewVariables]
    public SlotDefinition[] Slots = Array.Empty<SlotDefinition>();

    [ViewVariables]
    public ContainerSlot[] Containers = Array.Empty<ContainerSlot>();

    [DataField, AutoNetworkedField]
    public Dictionary<string, DisplacementData> Displacements = new();

    /// <summary>
    /// Alternate displacement maps, which if available, will be selected for the player of the appropriate gender.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<string, DisplacementData> FemaleDisplacements = new();

    /// <summary>
    /// Alternate displacement maps, which if available, will be selected for the player of the appropriate gender.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<string, DisplacementData> MaleDisplacements = new();
    
    //Starlight begin
    /// <summary>
    /// Force the stupid fucking client clothing system to use displacements, disregarding species ID.
    /// </summary>
    [DataField, AutoNetworkedField] public bool ForceDisplacements;
    //Starlight end
}

/// <summary>
/// Raised if the <see cref="InventoryComponent.TemplateId"/> of an inventory changed.
/// </summary>
[ByRefEvent]
public struct InventoryTemplateUpdated;

//Starlight begin
/// <summary>
/// Raised if <see cref="InventoryComponent.CustomSlots"/> gets updated via vv.
/// </summary>
[ByRefEvent]
public struct CustomInventorySlotsUpdated;
//Starlight end