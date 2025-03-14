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

    if (Commands.TryGetValue(command, out var button)) {
      button.ButtonPressed = true;
      button.GrabFocus();
    }
  }

  public void ClearCommands() {
    foreach (var child in CommandList.GetChildren()) {
      child.QueueFree();
    }

    Commands.Clear();
  }
  public void AddCommand(Command command) {
    var button = new Button() {
      Alignment = HorizontalAlignment.Center,
      ToggleMode = true,
      Icon = command.Data.Icon,
      ExpandIcon = true,
      ButtonGroup = _buttonGroup,
      CustomMinimumSize = new(32, 32),
      TooltipText = command.Data.CommandName
    };

    button.Toggled += (toggledOn) => EmitSignal(SignalName.CommandSelected, command);

    CommandList.AddChild(button);
    Commands.Add(command, button);
  }

  public int GetCommandCount() => Commands.Count;
}
