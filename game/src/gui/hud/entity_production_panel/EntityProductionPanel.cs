namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class EntityProductionPanel : Control {
  [Signal] public delegate void EntitySelectedEventHandler(EntityProductionAbilityResource.Instance entityData);
  [Export] private Control EntitiesList { get; set; }

  private ButtonGroup _buttonGroup;
  public override void _Ready() {
    _buttonGroup = new();
  }

  public void ClearEntities() {
    foreach (var child in EntitiesList.GetChildren()) {
      child.Free();
    }
  }
  public void AddProducedEntity(EntityProductionAbilityResource.Instance ability, IMatchPlayerState playerState) {
    var productionConfig = (ability.AbilityResource as EntityProductionAbilityResource).ProductionConfig;
    var entityData = playerState.GetEntity(productionConfig.EntityTag);

    var text = string.Format($"{entityData.EntityName}\nRounds to Produce: {productionConfig.ProductionTime}");
    var button = new Button() {
      Alignment = HorizontalAlignment.Left,
      Text = text,
      ButtonGroup = _buttonGroup,
    };

    button.Pressed += () => EmitSignal(SignalName.EntitySelected, ability);

    EntitiesList.AddChild(button);
  }

  public int GetEntriesCount() => EntitiesList.GetChildCount();
}
