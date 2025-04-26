namespace HOB;

using Godot;
using System;
using WidgetSystem;

public partial class CommandWidget : HOBWidget, IWidgetFactory<CommandWidget> {
  [Export] public CommandButtonWidget ButtonWidget { get; private set; } = default!;
  public static CommandWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://s6oh5eah6obb").Instantiate<CommandWidget>();
  }

  public void BindTo(HOBAbility.Instance ability) {
    ButtonWidget.BindAbility(ability);
  }
}
