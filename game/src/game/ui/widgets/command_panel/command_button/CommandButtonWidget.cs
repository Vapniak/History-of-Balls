namespace HOB;

using Godot;
using WidgetSystem;

[GlobalClass]
public partial class CommandButtonWidget : ButtonWidget, IWidgetFactory<CommandButtonWidget> {
  public static CommandButtonWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://s6oh5eah6obb").Instantiate<CommandButtonWidget>();
  }

  public void BindAbility(HOBAbility.Instance ability) {
    ((Button)Button).Icon = ability.AbilityResource.Icon;
  }

  public override GodotObject _MakeCustomTooltip(string forText) {
    return TooltipWidget.CreateWidget().Configure(label => {
      label.AppendText(forText);
    });
  }
}
