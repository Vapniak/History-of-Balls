namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Entity : Node3D {
  public GameCell Cell { get; set; }

  private readonly Dictionary<Type, Trait> _traits = new();

  public override void _Ready() {
    foreach (var child in GetChildren()) {
      if (child is Trait trait) {
        trait.Owner = this;
        _traits.Add(trait.GetType(), trait);
      }
    }
  }

  public bool TryGetTrait<T>(out T trait) where T : Trait {
    if (_traits.TryGetValue(typeof(T), out var t)) {
      trait = t as T;
      return true;
    }

    trait = null;
    return false;
  }
}
