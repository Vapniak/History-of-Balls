namespace HOB.GameEntity;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ActionsTrait : Trait {
  [Export] public Array<Action> Actions { get; private set; }
  [Export] private ActionMenu ActionsMenu { get; set; }

  public override void _Ready() {
    base._Ready();

    HideActionMenu();

    foreach (var action in Actions) {
      action.OwnerTrait = this;
      ActionsMenu.AddAction(action);
    }
  }

  public void ShowActionMenu() {
    ActionsMenu.Show();
  }
  public void HideActionMenu() => ActionsMenu.Hide();
}
