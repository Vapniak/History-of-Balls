namespace HOB;

using Godot;
using Godot.Collections;
using System;

public partial class GameEntity : Node3D {
  public override void _Ready() {
    InitComponents();
  }

  public T GetComponent<T>() where T : GameEntityComponent {
    foreach (var comp in GetAllComponents()) {
      if (comp is T) {
        return comp as T;
      }
    }

    return null;
  }

  public Array<GameEntityComponent> GetAllComponents() {
    var components = new Array<GameEntityComponent>();
    foreach (var child in GetChildren()) {
      if (child is GameEntityComponent comp) {
        components.Add(comp);
      }
    }

    return components;
  }

  private void InitComponents() {
    foreach (var comp in GetAllComponents()) {
      comp.Owner = this;
    }
  }
}
