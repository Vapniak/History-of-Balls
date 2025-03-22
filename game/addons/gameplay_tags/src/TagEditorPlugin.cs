using GameplayTags;
using Godot;

[Tool]
public partial class TagEditorPlugin : EditorInspectorPlugin {
  public override bool _CanHandle(GodotObject @object) {
    return true;
  }

  public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide) {
    if (type == Variant.Type.Object && hintString == "Tag") {
      AddPropertyEditor(name, new TagPropertyEditor());
      return true;
    }
    return false;
  }
}