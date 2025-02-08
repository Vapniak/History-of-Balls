namespace HOB;

using Godot;
using Godot.Collections;

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

  public static Array<Node> GetAllChildren(this Node node) {
    Array<Node> children = new();

    foreach (var child in node.GetChildren()) {
      children.Add(child);
      if (child.GetChildCount() > 0) {
        children.AddRange(GetAllChildren(child));
      }
    }

    return children;
  }
}
