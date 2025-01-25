namespace HOB;


using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class ActionMenu : Control {
  [Export] private Control ActionList { get; set; }

  [Export] private PackedScene ActionListItem { get; set; }

  private Dictionary<string, Control> ActionNameToListItem { get; set; }

  public override void _Ready() {
    foreach (var child in ActionList.GetChildren()) {
      child.QueueFree();
    }

    ActionNameToListItem = new();
  }

  public void AddAction(Action action) {
    var item = ActionListItem.Instantiate<Control>();
    ActionNameToListItem.Add(action.Name, item);

    item.GetNode<Label>("Label").Text = action.Name;
    item.GetNode<Button>("Button").Pressed += action.Start;
    ActionList.AddChild(item);
  }

  public void RemoveAction(Action action) {
    if (ActionNameToListItem.TryGetValue(action.Name, out var item)) {
      item.QueueFree();
    }
  }
}
