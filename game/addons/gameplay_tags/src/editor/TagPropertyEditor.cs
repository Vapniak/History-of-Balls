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

    _button = new Button();
    AddChild(_button);

    _button.Pressed += ShowTree;
    //UpdateProperty();
    CreatePopupTree();
  }

  private void CreatePopupTree() {
    _tree = new Tree() {
      SizeFlagsHorizontal = SizeFlags.ExpandFill,
      SizeFlagsVertical = SizeFlags.ExpandFill,
      CustomMinimumSize = new Vector2I(0, 100),
      Columns = 1,
      HideRoot = true
    };

    _tree.ItemActivated += OnItemActivated;

    _tree.Hide();
    AddChild(_tree);

    BuildTagTree();
  }

  private void BuildTagTree() {
    _tree?.Clear();
    _treeItems.Clear();
    var root = _tree?.CreateItem();


    if (_tree != null) {
      foreach (var tag in TagManager.Tags) {
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
    _tree?.Show();
  }

  private void OnItemActivated() {
    ConfirmSelection();
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

      var tag = TagManager.GetTag(fullName);
      _currentValue = tag.FullName;
      UpdateButtonText();
      EmitChanged(GetEditedProperty(), tag);
      _tree?.Hide();
    }
  }

  public override void _UpdateProperty() {
    if (GetEditedObject() != null && GetEditedProperty() != null) {
      var value = GetEditedObject().Get(GetEditedProperty());

      if (value.As<GodotObject>() is Tag tag) {
        _currentValue = tag.FullName;
      }
      UpdateButtonText();
    }
  }

  private void UpdateButtonText() {
    if (_button != null) {
      _button.Text = _currentValue;
    }
  }
}
