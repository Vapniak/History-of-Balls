namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class EntityProductionPanel : Control {
  [Signal] public delegate void EntitySelectedEventHandler(int index);
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
  public void AddProducedEntity(ProducedEntityData data, bool canBeProduced) {
    var tooltip = string.Format("Cost: {0}\nProduction Time Rounds: {1}", data.Cost, data.RoundsProductionTime);
    var button = new Button() {
      Disabled = !canBeProduced,
      Alignment = HorizontalAlignment.Left,
      Text = data.Entity.EntityName,
      ButtonGroup = _buttonGroup,
      TooltipText = tooltip,
    };

    button.Pressed += () => EmitSignal(SignalName.EntitySelected, button.GetIndex());

    Entities.Add(data);
    EntitiesList.AddChild(button);
  }
}
