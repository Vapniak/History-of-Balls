namespace HOB;

using Godot;
using WidgetSystem;

[GlobalClass]
public partial class CommandButtonWidget : Widget, IWidgetFactory<CommandButtonWidget> {
  [Export] public Button Button { get; private set; } = default!;

  public override void _Ready() {
    FocusEntered += Button.GrabFocus;
  }

  public static CommandButtonWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://s6oh5eah6obb").Instantiate<CommandButtonWidget>();
  }

  public void BindAbility(HOBAbilityInstance ability) {
    Button.Icon = ability.AbilityResource.Icon;
  }

  public override GodotObject _MakeCustomTooltip(string forText) {
    return TooltipWidget.CreateWidget().Configure(label => {
      label.AppendText(forText);
    });
  }
}
