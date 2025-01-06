namespace HOB.GameEntity;

using GameplayFramework;
using Godot;
using HexGridMap;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Entity : Node3D {
  public PlayerState OwnerState { get; set; }
  public HexCoordinates Coordinates { get; set; }

  private readonly Dictionary<Type, Trait> _traits = new();

  public override void _Ready() {
    foreach (var child in GetChildren()) {
      if (child is Trait trait) {
        trait.Owner = this;
        AddTrait(trait);
      }
    }
  }

  public T GetTrait<T>() where T : Trait {
    return (T)_traits[typeof(T)];
  }

  private void AddTrait<T>(T trait) where T : Trait {
    _traits[typeof(T)] = trait;
  }
}
