namespace HOB;

using Godot;
using Godot.Collections;
using HOB.GameEntity;
using System.Linq;

public partial class CommandPanelWidget : Control {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbility.Instance abilityInstance);
  [Export] private Control CommandList { get; set; } = default!;

  private Dictionary<HOBAbility.Instance, CommandWidget> Commands { get; set; } = new();

  private Entity? CurrentEntity { get; set; }

  public void Initialize(HOBPlayerController playerController) {
    Hide();

    playerController.SelectedCommandChanged += () => {
      if (playerController.SelectedCommand != null) {
        SelectCommand(playerController.SelectedCommand);
      }
    };

    playerController.SelectedEntityChanged += () => {
      var entity = playerController.SelectedEntity;

      if (entity != null) {
        BindTo(entity);
      }
      else {
        Unbind();
      }
    };
  }

  private void BindTo(Entity entity) {
    CurrentEntity = entity;

    ClearCommands();
    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities().OrderBy(a => (a.AbilityResource as HOBAbility)?.UIOrder)) {
      if (ability is HOBAbility.Instance hOBAbility && hOBAbility.AbilityResource.ShowInUI) {
        AddCommand(hOBAbility);
      }
    }

    if (GetCommandCount() > 0) {
      Show();
    }
  }

  private void Unbind() {
    CurrentEntity = null;
    Hide();
  }

  private void SelectCommand(int index) {
    if (index >= 0 && index < Commands.Count) {
      var command = Commands.ElementAt(index);
      SelectCommand(command.Key);
    }
  }

  private void SelectCommand(HOBAbility.Instance command) {
    if (command == null) {
      return;
    }

    if (Commands.TryGetValue(command, out var button)) {
      button.ButtonWidget.Button.EmitSignal(Button.SignalName.Pressed);
      button.ButtonWidget.Button.GrabFocus();
    }
  }

  private void ClearCommands() {
    foreach (var child in CommandList.GetChildren()) {
      child.Free();
    }

    Commands.Clear();
  }
  private void AddCommand(HOBAbility.Instance ability) {
    var buttonWidget = CommandWidget.CreateWidget();
    buttonWidget.BindTo(ability);
    var button = buttonWidget;
    button.ButtonWidget.Button.Pressed += () => EmitSignal(SignalName.CommandSelected, ability);

    CommandList.AddChild(buttonWidget);
    Commands.Add(ability, buttonWidget);
  }

  private int GetCommandCount() => Commands.Count;
}
