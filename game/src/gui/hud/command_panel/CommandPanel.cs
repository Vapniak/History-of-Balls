namespace HOB;

using GameplayAbilitySystem;
using Godot;
using Godot.Collections;
using HOB.GameEntity;
using System.Linq;
using Tooltip;

public partial class CommandPanel : Control {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbilityInstance abilityInstance);
  [Export] private Control CommandList { get; set; }

  private Dictionary<HOBAbilityInstance, Button> Commands { get; set; }
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

  public void SelectCommand(HOBAbilityInstance command) {
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
  public void AddCommand(HOBAbilityInstance ability) {
    var button = new Button() {
      Alignment = HorizontalAlignment.Center,
      ToggleMode = true,
      Icon = ability.AbilityResource.Icon,
      IconAlignment = HorizontalAlignment.Center,
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      ExpandIcon = true,
      ButtonGroup = _buttonGroup,
      CustomMinimumSize = new(32, 32),
      ThemeTypeVariation = "ActionButton",
    };

    //button.AddChild(new TooltipTrigger(command.AbilityResource.AbilityName));

    button.Toggled += (toggledOn) => EmitSignal(SignalName.CommandSelected, ability);

    CommandList.AddChild(button);
    Commands.Add(ability, button);
  }

  public int GetCommandCount() => Commands.Count;
}
