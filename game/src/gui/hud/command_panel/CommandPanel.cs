namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;
using System.Linq;

public partial class CommandPanel : Control {
  [Signal] public delegate void CommandSelectedEventHandler(Command command);
  [Export] private Control CommandList { get; set; }

  private Dictionary<Command, Button> Commands { get; set; }
  private ButtonGroup _buttonGroup;
  public override void _Ready() {
    _buttonGroup = new();

    Commands = new();
  }


  public void SelectCommand(int index) {
    if (index >= 0 && index < Commands.Count) {
      var command = Commands.ElementAt(index);
      SelectCommand(command.Key);
    }
  }

  public void SelectCommand(Command command) {
    if (command == null) {
      return;
    }

    if (!CommandCanBeSelected(command)) {
      return;
    }

    var button = Commands[command];
    button.ButtonPressed = true;
    button.GrabFocus();
  }

  public void ClearCommands() {
    foreach (var child in CommandList.GetChildren()) {
      child.QueueFree();
    }

    Commands.Clear();
  }
  public void AddCommand(Command command) {
    var button = new Button() {
      Disabled = !CommandCanBeSelected(command),
      Alignment = HorizontalAlignment.Left,
      ToggleMode = true,
      Text = command.CommandName,
      ButtonGroup = _buttonGroup,
    };

    button.Toggled += (toggledOn) => EmitSignal(SignalName.CommandSelected, command);

    CommandList.AddChild(button);
    Commands.Add(command, button);
  }

  public int GetCommandCount() => Commands.Count;

  public static bool CommandCanBeSelected(Command command) {
    return command.IsAvailable() || (command.GetEntity().TryGetOwner(out var owner) && !owner.IsCurrentTurn());
  }
}
