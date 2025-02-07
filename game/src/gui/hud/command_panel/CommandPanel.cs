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

    FocusEntered += CommandList.GrabFocus;
    Commands = new();

    CommandList.ItemSelected += (index) => {
      if (CommandList.IsItemSelectable((int)index)) {
        EmitSignal(SignalName.CommandSelected, Commands[(int)index]);
      }
    };
  }


  public void SelectCommand(int index) {
    if (index >= 0 && index < CommandList.ItemCount) {
      CommandList.Select(index);
      CommandList.EmitSignal(ItemList.SignalName.ItemSelected, index);
    }
  }

  public void SelectCommand(Command command) {
    var index = Commands.IndexOf(command);
    SelectCommand(index);
  }

  public void ClearCommands() {
    CommandList.Clear();
    Commands.Clear();
  }
  public void AddCommand(Command command) {
    var text = command.CommandName;
    if (CommandList.ItemCount <= 4) {
      text = CommandList.ItemCount + 1 + " | " + command.CommandName;
    }
    CommandList.AddItem(text, null, command.IsAvailable() && command.CommandTrait.CurrentExecutedCommand == null);
    Commands.Add(command);
  }
}
