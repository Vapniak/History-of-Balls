namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class EntityProductionPanel : Control {
  [Signal] public delegate void EntitySelectedEventHandler(ProducedEntityData entityData);
  [Export] private Control EntitiesList { get; set; }

  private Array<ProducedEntityData> Entities { get; set; }
  private ButtonGroup _buttonGroup;
  public override void _Ready() {
    _buttonGroup = new();

    Entities = new();
  }

  public void ClearEntities() {
    foreach (var child in EntitiesList.GetChildren()) {
      child.QueueFree();
    }

    Entities.Clear();
  }
  public void AddProducedEntity(ProducedEntityData data, IMatchPlayerState playerState, bool canBeProduced) {
    var text = string.Format($"{data.Entity.EntityName}\n\nCost: {data.Cost} {playerState.GetResourceType(data.CostType).Name}\nRounds to Produce: {data.RoundsProductionTime}");
    var button = new Button() {
      Disabled = !canBeProduced,
      Alignment = HorizontalAlignment.Left,
      Text = text,
      ButtonGroup = _buttonGroup,
    };

    button.Pressed += () => EmitSignal(SignalName.EntitySelected, data);

    Entities.Add(data);
    EntitiesList.AddChild(button);
  }
}
