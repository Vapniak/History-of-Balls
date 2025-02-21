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
    //CommandList.Clear();

    _buttonGroup = new() {
      AllowUnpress = true,
    };

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
      foreach (var b in _buttonGroup.GetButtons()) {
        b.ButtonPressed = false;
      }
      return;
    }

    var button = Commands[command];
    button.ButtonPressed = !button.ButtonPressed;
    button.GrabFocus();
  }

  public void ClearCommands() {
    foreach (var child in CommandList.GetChildren()) {
      child.QueueFree();
    }

    Commands.Clear();
  }
  public void AddCommand(Command command) {
    var text = command.CommandName;
    if (Commands.Count <= 4) {
      text = Commands.Count + 1 + " | " + command.CommandName;
    }
    var button = new Button() {
      Disabled = !command.IsAvailable(),
      Alignment = HorizontalAlignment.Left,
      ToggleMode = true,
      Text = text,
      ButtonGroup = _buttonGroup,
    };

    button.Toggled += (toggledOn) => EmitSignal(SignalName.CommandSelected, toggledOn && command.IsAvailable() ? command : null);

    CommandList.AddChild(button);
    Commands.Add(command, button);
  }

  public int GetCommandCount() => Commands.Count;
}
