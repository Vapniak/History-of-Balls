namespace HOB.Entity;

using Godot;
using System;
using System.Collections.Generic;

public partial class GameEntity : Node3D {
  [Export] public GameEntityData GameEntityData { get; private set; }

  private readonly Dictionary<Type, GameEntityTrait> _traits = new();

  public override void _Ready() {
    foreach (var child in GetChildren()) {
      if (child is GameEntityTrait trait) {
        trait.Owner = this;
        trait.SetData(GameEntityData);
        AddTrait(trait);
      }
    }
  }

  public T GetTrait<T>() where T : GameEntityTrait {
    return (T)_traits[typeof(T)];
  }

  private void AddTrait<T>(T trait) where T : GameEntityTrait {
    _traits[typeof(T)] = trait;
  }
}
