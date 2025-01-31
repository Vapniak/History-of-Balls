namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Entity : Node3D {
  [Export] public string EntityName { get; private set; }
  public GameCell Cell { get; set; }

  public IMatchController OwnerController { get; set; }

  private readonly Dictionary<Type, Trait> _traits = new();

  public override void _EnterTree() {
    foreach (var child in GetChildren()) {
      if (child is Trait trait) {
        trait.Entity = this;
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

  public T GetTrait<T>() where T : Trait {
    return _traits.GetValueOrDefault(typeof(T)) as T;
  }
}
