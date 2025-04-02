namespace HOB;

using System.Collections.Generic;
using System.Diagnostics;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class EntityProductionPanel : Control {
  [Export] private Control? EntitiesList { get; set; }
  private ButtonGroup? _buttonGroup;
  private EntityProductionAbilityResource.Instance? _ability;

  private Dictionary<ProductionConfig, Button> ProductionToButton { get; set; } = new();
  public override void _Ready() {
    _buttonGroup = new();

    Hidden += () => {
      ClearEntities();
    };
  }

  public void ClearEntities() {
    if (EntitiesList == null) {
      return;
    }

    ProductionToButton?.Clear();
    _ability = null;
    foreach (var child in EntitiesList.GetChildren()) {
      child.Free();
    }
  }

  public void ShowProducedEntities(EntityProductionAbilityResource.Instance ability, IMatchPlayerState playerState) {
    ClearEntities();

    _ability = ability;

    foreach (var config in playerState.ProducedEntities) {
      var costText = "";
      if (config.CostEffect?.EffectDefinition?.Modifiers != null) {
        foreach (var modifier in config.CostEffect.EffectDefinition.Modifiers) {
          var ei = ability.OwnerAbilitySystem.MakeOutgoingInstance(config.CostEffect, 0);
          costText += $"\n    {modifier.GetMagnitude(ei) * -1} {modifier.Attribute.AttributeName}";
        }
      }

      var text = string.Format($"{config?.Entity?.EntityName}\nCost: {costText}\nRounds to Produce: {config?.ProductionTime}");
      var button = new Button() {
        Alignment = HorizontalAlignment.Left,
        Text = text,
        Icon = GameInstance.GetGameMode<HOBGameMode>().GetIconFor(config.Entity),
        ExpandIcon = false,
        ButtonGroup = _buttonGroup,
      };

      button.Pressed += () => ability.OwnerAbilitySystem.TryActivateAbility(ability, new() { Activator = playerState.GetController(), TargetData = new() { Target = config } });

      ProductionToButton?.TryAdd(config, button);
      EntitiesList?.AddChild(button);
    }

    if (ProductionToButton?.Count > 0) {
      Show();
    }
  }

  public int GetEntriesCount() => EntitiesList?.GetChildCount() ?? 0;

  public override void _PhysicsProcess(double delta) {
    if (_ability == null) {
      return;
    }

    if (_ability.OwnerAbilitySystem.Owner is Entity entity && entity.TryGetOwner(out var owner)) {
      foreach (var (config, button) in ProductionToButton) {
        button.Disabled = !_ability.CanActivateAbility(new() { Activator = owner, TargetData = new() { Target = config } });
      }
    }
  }
}
