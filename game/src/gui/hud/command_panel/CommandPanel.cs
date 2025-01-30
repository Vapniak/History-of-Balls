namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;
using System;

public partial class CommandPanel : Control {
  [Signal] public delegate void CommandSelectedEventHandler(Command command);
  [Export] private ItemList CommandList { get; set; }

  private Array<Command> Commands { get; set; }
  public override void _Ready() {
    //CommandList.Clear();
    Commands = new();

    CommandList.ItemSelected += (index) => EmitSignal(SignalName.CommandSelected, Commands[(int)index]);
  }

  public void SelectCommand(Command command) {
    var index = Commands.IndexOf(command);
    if (index >= 0) {
      CommandList.Select(index);
      CommandList.EmitSignal(ItemList.SignalName.ItemSelected, index);
    }
  }

  public void ClearCommands() {
    CommandList.Clear();
    Commands.Clear();
  }
  public void AddCommand(Command command) {
    CommandList.AddItem(command.Name, null, command.IsAvailable());
    Commands.Add(command);
  }
}
