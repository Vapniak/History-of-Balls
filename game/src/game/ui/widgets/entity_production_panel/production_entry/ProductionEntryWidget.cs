namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ProductionEntryWidget : ButtonWidget, IWidgetFactory<ProductionEntryWidget> {
  [Export] private Label EntityName { get; set; } = default!;
  [Export] private EntityIconWidget EntityIcon { get; set; } = default!;
  [Export] private Label RoundsToProduceLabel { get; set; } = default!;
  [Export] private RichTextLabel CostLabel { get; set; } = default!;

  private IMatchController? _boundController;
  private EntityProductionAbility.Instance? _boundAbility;
  private ProductionConfig? _boundProductionConfig;


  public override void _EnterTree() {
    Button.Pressed += () => _boundAbility?.OwnerAbilitySystem.TryActivateAbility(_boundAbility, new() { Activator = _boundController, TargetData = new() { Target = _boundProductionConfig } });

  }
  public static ProductionEntryWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://csmsivxcvng13").Instantiate<ProductionEntryWidget>();
  }

  public void BindTo(IMatchController playerController, EntityProductionAbility.Instance ability, ProductionConfig productionConfig) {
    _boundController = playerController;
    _boundAbility = ability;
    _boundProductionConfig = productionConfig;


    var costText = "";
    var ei = ability.OwnerAbilitySystem.MakeOutgoingInstance(productionConfig.CostEffect, 0, playerController.GetPlayerState().AbilitySystem);

    var aggregators = ei.GetAggregators();
    CostLabel.Text = "";
    while (aggregators.MoveNext()) {
      CostLabel.AppendText($"{aggregators.Current.aggregator.SumMods() * -1} {aggregators.Current.attribute.AttributeName}\n");
    }


    EntityName.Text = productionConfig.Entity.EntityName;
    EntityIcon.SetIcon(productionConfig.Entity.Icon);

    RoundsToProduceLabel.Text = productionConfig.ProductionTime.ToString();

  }

  public override void _PhysicsProcess(double delta) {
    if (_boundAbility == null || _boundController == null || _boundProductionConfig == null) {
      return;
    }

    if (_boundAbility.OwnerEntity.TryGetOwner(out var owner)) {
      var canActivate = _boundAbility.CanActivateAbility(new() {
        Activator = _boundController,
        TargetData = new() { Target = _boundProductionConfig }
      });
      Button.Disabled = !canActivate;

      // if (!canActivate) {
      //   Label.AddThemeColorOverride("default_color", Colors.Gray);
      //   Label.UpdateImage("img", RichTextLabel.ImageUpdateMask.Color, _boundProductionConfig.Entity.Icon, color: Colors.Gray);
      // }
      // else {
      //   Label.UpdateImage("img", RichTextLabel.ImageUpdateMask.Color, _boundProductionConfig.Entity.Icon, color: Colors.White);
      //   Label.RemoveThemeColorOverride("default_color");
      // }
    }
  }
}
