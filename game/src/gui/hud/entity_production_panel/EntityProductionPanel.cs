namespace HOB;

using GameplayFramework;
using Godot;

public partial class EntityProductionPanel : Control {
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
  public void AddProducedEntity(EntityProductionAbilityResource.Instance ability, ProductionConfig productionConfig, IMatchPlayerState playerState) {
    var costText = "";
    foreach (var modifier in productionConfig.CostEffect.EffectDefinition.Modifiers) {
      var ei = ability.OwnerAbilitySystem.MakeOutgoingInstance(productionConfig.CostEffect, 0);
      costText += $"\n    {modifier.GetMagnitude(ei) * -1} {modifier.Attribute.AttributeName}";
    }

    var text = string.Format($"{productionConfig.Entity.EntityName}\nCost: {costText}\nRounds to Produce: {productionConfig.ProductionTime}");
    var button = new Button() {
      Alignment = HorizontalAlignment.Left,
      Text = text,
      Icon = playerState.GetController<PlayerController>().GetHUD<HOBHUD>().GetIconFor(productionConfig.Entity),
      ExpandIcon = false,
      ButtonGroup = _buttonGroup,
    };

    button.Pressed += () => ability.OwnerAbilitySystem.TryActivateAbility(ability, new() { Activator = playerState.GetController(), TargetData = new() { Target = productionConfig } });

    EntitiesList.AddChild(button);
  }

  public int GetEntriesCount() => EntitiesList.GetChildCount();
}
