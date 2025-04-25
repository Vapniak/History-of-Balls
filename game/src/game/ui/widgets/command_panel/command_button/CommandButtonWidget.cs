namespace HOB;

using Godot;
using WidgetSystem;

[GlobalClass]
public partial class CommandButtonWidget : ButtonWidget, IWidgetFactory<CommandButtonWidget> {
  public static CommandButtonWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://s6oh5eah6obb").Instantiate<CommandButtonWidget>();
  }

  public void BindAbility(HOBAbility.Instance ability) {
    Button.Icon = ability.AbilityResource.Icon;
  }
}
