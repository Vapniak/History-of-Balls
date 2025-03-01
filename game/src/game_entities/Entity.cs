namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class Entity : Node {
  public EntityBody Body { get; private set; }

  [Notify]
  public GameCell Cell {
    get => _cell.Get();
    set => _cell.Set(value);
  }
  public GameBoard GameBoard { get; private set; }

  private EntityData Data { get; set; }

  [Notify]
  private IMatchController OwnerController { get => _ownerController.Get(); set => _ownerController.Set(value); }
  private readonly Dictionary<Type, Trait> _traits = new();

  public Entity(EntityData data, GameCell cell, GameBoard gameBoard) {
    Cell = cell;
    GameBoard = gameBoard;
    Data = data;
  }

  public override void _EnterTree() {
    Body = Data.Body.Instantiate<EntityBody>();
    AddChild(Body);

    Data = Data.Duplicate(true) as EntityData;
    Data.Stats.Init();

    var traits = Data.TraitsScene.Instantiate<Node>();

    foreach (var child in traits.GetAllChildren()) {
      if (child is Trait trait) {
        trait.Entity = this;
        _traits.Add(trait.GetType(), trait);
      }
    }

    AddChild(traits);

    CallDeferred(MethodName.SetPosition, Cell.GetRealPosition());
  }

  public string GetEntityName() => Data.EntityName;

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

  public bool TryGetStat<T>(out T stat) where T : BaseStat {
    return Data.Stats.TryGetStat(out stat);
  }
  public T GetStat<T>() where T : BaseStat {
    return Data.Stats.GetStat<T>();
  }

  public void SetPosition(Vector3 position) {
    Body.GlobalPosition = position;
  }

  public bool TryGetOwner(out IMatchController owner) {
    owner = OwnerController;
    return owner != null;
  }

  public Vector3 GetPosition() => Body.GlobalPosition;

  public async Task TurnAt(Vector3 targetPosition, float duration) {
    var targetRotation = Basis.LookingAt(GetPosition().DirectionTo(targetPosition) * new Vector3(1, 0, 1)).GetRotationQuaternion();

    var tween = CreateTween();
    tween.TweenProperty(Body, "quaternion", targetRotation, duration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

    await ToSignal(tween, Tween.SignalName.Finished);
  }

  public void SetMaterial(Material material) {
    foreach (var child in Body.GetAllChildren()) {
      if (child is MeshInstance3D meshInstance) {
        meshInstance.MaterialOverlay = material;
      }
    }
  }

  public void ChangeOwner(IMatchController newOwner) {
    if (newOwner != OwnerController) {
      OwnerController = newOwner;
    }
  }
}
