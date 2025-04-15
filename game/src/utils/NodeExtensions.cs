namespace HOB;

using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public static class NodeExtensions {
  public static T? GetChildByType<T>(this Node node) where T : Node {
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

  public static Array<T> GetChildrenByType<[MustBeVariant] T>(this Node parent) where T : Node {
    var result = new Array<T>();
    var queue = new Queue<Node>();
    queue.Enqueue(parent);

    while (queue.Count > 0) {
      var node = queue.Dequeue();
      if (node is T matchedNode) {
        result.Add(matchedNode);
      }
      foreach (var child in node.GetChildren()) {
        queue.Enqueue(child);
      }
    }
    return result;
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
