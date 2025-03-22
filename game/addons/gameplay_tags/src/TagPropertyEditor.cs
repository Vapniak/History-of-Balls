namespace GameplayTags;

using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class TagPropertyEditor : EditorProperty {
  private Button? _button;
  private Tree? _tree;
  private string? _currentValue;
  private readonly Dictionary<string, TreeItem> _treeItems = new();

  public override void _Ready() {
    base._Ready();

    _button = new Button { Text = "Select Tag" };
    _button.Connect(Button.SignalName.Pressed, Callable.From(ShowTree));
    AddChild(_button);

    CreatePopupTree();
  }

  private void CreatePopupTree() {
    _tree = new Tree() {
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      SizeFlagsVertical = SizeFlags.ExpandFill,
      CustomMinimumSize = new Vector2I(300, 400),
      Columns = 1,
      HideRoot = true
    };
    _tree.ItemSelected += OnItemSelected;
    _tree.ItemActivated += OnItemActivated;

    _tree.Hide();
    AddChild(_tree);
  }

  private void BuildTagTree() {
    _tree?.Clear();
    _treeItems.Clear();
    var root = _tree?.CreateItem();


    if (_tree != null) {
      foreach (var tag in TagsManager.Tags) {
        var parts = tag.Key.Split('.');
        var parent = root;

        for (var i = 0; i < parts.Length; i++) {
          var path = string.Join(".", parts.Take(i + 1));
          if (!_treeItems.TryGetValue(path, out var current)) {
            current = _tree.CreateItem(parent);
            current.SetText(0, parts[i]);
            current.SetMetadata(0, path);
            _treeItems[path] = current;
          }
          parent = current;
        }
      }
    }
  }

  private void ShowTree() {
    BuildTagTree();
    _tree?.Show();
  }

  private void OnItemSelected() {
    UpdateSelection(_tree?.GetSelected());
  }

  private void OnItemActivated() {
    ConfirmSelection();
  }

  private void UpdateSelection(TreeItem? item) {
    if (item != null) {
      _currentValue = item.GetMetadata(0).AsString();
    }
  }

  private void ConfirmSelection() {
    var selected = _tree?.GetSelected();
    if (selected != null) {
      var parts = new List<string>();
      var current = selected;
      while (current != null && current.GetText(0) != "") {
        parts.Insert(0, current.GetText(0));
        current = current.GetParent();
      }
      var fullName = string.Join(".", parts);

      var tag = TagsManager.GetTag(fullName);
      if (tag != Tag.Empty) {

        EmitChanged(GetEditedProperty(), tag);
        if (_button != null) {
          _button.Text = fullName.Split('.').Last();
        }
      }
      _tree?.Hide();
    }
  }

  public override void _UpdateProperty() {
    var value = GetEditedObject().Get(GetEditedProperty());

    if (_button != null) {
      if (value.As<Resource>() is Tag tag) {
        GD.Print($"Tag.FullName: {tag.FullName}");
        _button.Text = tag.FullName ?? "Select Tag";
      }
      else {
        GD.Print("Value is not a valid Tag resource!");
        _button.Text = "Select Tag";
      }
    }
  }
}
