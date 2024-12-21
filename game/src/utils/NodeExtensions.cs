namespace HOB;

using Godot;

public static class NodeExtensions {
  public static T GetChildByType<T>(this Node node) where T : Node {
    for (var i = 0; i < node.GetChildCount(); i++) {
      var child = node.GetChild(i);
      if (child is T) {
        return child as T;
      }

      if (child.GetChildCount() > 0) {
        return child.GetChildByType<T>();
      }
    }

    return null;
  }
}
