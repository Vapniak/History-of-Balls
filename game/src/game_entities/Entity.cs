namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Entity : Node {
  [Export] public string EntityName { get; private set; }
  [Export] private Node3D Body { get; set; }
  [Export] private Node TraitsContainer { get; set; }
  public GameCell Cell { get; set; }

  public IMatchController OwnerController { get; set; }

  private readonly Dictionary<Type, Trait> _traits = new();

  public override void _EnterTree() {
    foreach (var child in TraitsContainer.GetAllChildren()) {
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

  public bool IsOwnedBy(IMatchController controller) {
    return controller == OwnerController;
  }

  public void SetPosition(Vector3 position) {
    Body.GlobalPosition = position;
  }
  public Vector3 GetPosition() => Body.GlobalPosition;

  public void LookAt(Vector3 pos) {
    Body.LookAt(pos);
  }
}
