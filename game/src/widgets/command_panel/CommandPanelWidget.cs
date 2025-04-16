namespace HOB;

using Godot;
using Godot.Collections;
using System.Linq;

public partial class CommandPanelWidget : Control {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbilityInstance abilityInstance);
  [Export] private Control CommandList { get; set; } = default!;

  private Dictionary<HOBAbilityInstance, CommandButtonWidget> Commands { get; set; } = new();


  public void SelectCommand(int index) {
    if (index >= 0 && index < Commands.Count) {
      var command = Commands.ElementAt(index);
      SelectCommand(command.Key);
    }
  }

  public void SelectCommand(HOBAbilityInstance command) {
    if (command == null) {
      return;
    }

    if (Commands.TryGetValue(command, out var button)) {
      button.Button.ButtonPressed = true;
      button.GrabFocus();
    }
  }

  public void ClearCommands() {
    foreach (var child in CommandList.GetChildren()) {
      child.Free();
    }

    Commands.Clear();
  }
  public void AddCommand(HOBAbilityInstance ability) {
    var buttonWidget = CommandButtonWidget.CreateWidget();
    buttonWidget.BindAbility(ability);
    var button = buttonWidget.Button;
    button.Toggled += (toggledOn) => EmitSignal(SignalName.CommandSelected, ability);

    CommandList.AddChild(buttonWidget);
    Commands.Add(ability, buttonWidget);
  }

  public int GetCommandCount() => Commands.Count;
}
