namespace HOB;

using Godot;
using HOB.GameEntity;
using System;

public partial class CommandPanel : Control {
  [Export] private ItemList CommandList { get; set; }

  public override void _Ready() {
    //CommandList.Clear();
  }

  public void ClearCommands() => CommandList.Clear();
  public void AddCommand(Command command) {
    CommandList.AddItem(command.Name, null, command.IsAvailable());
  }
}
