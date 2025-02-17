namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class Entity : Node {
  [Export] public Node3D Body { get; private set; }

  public GameCell Cell { get; set; }
  public IMatchController OwnerController { get; private set; }
  public GameBoard GameBoard { get; private set; }


  private EntityData Data { get; set; }

  private readonly Dictionary<Type, Trait> _traits = new();

  public override void _EnterTree() {
    var body = Data.Body.Instantiate<Node3D>();
    Body.AddChild(body);

    Data = Data.Duplicate(true) as EntityData;
    Data.Stats.Init();

    var traits = Data.TraitsScene.Instantiate<Node>();
    AddChild(traits);

    foreach (var child in traits.GetAllChildren()) {
      if (child is Trait trait) {
        trait.Entity = this;
        _traits.Add(trait.GetType(), trait);
      }
    }

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

  public bool IsOwnedBy(IMatchController controller) {
    return controller == OwnerController;
  }

  public void SetPosition(Vector3 position) {
    Body.GlobalPosition = position;
  }
  public Vector3 GetPosition() => Body.GlobalPosition;

  public async Task TurnAt(Vector3 targetPosition, float duration) {
    var targetRotation = Basis.LookingAt(GetPosition().DirectionTo(targetPosition) * new Vector3(1, 0, 1)).GetRotationQuaternion();

    var tween = CreateTween();
    tween.TweenProperty(Body, "quaternion", targetRotation, duration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);

    await ToSignal(tween, Tween.SignalName.Finished);
  }

  [OnInstantiate]
  private void Init(IMatchController owner, EntityData data, GameCell cell, GameBoard gameBoard) {
    OwnerController = owner;
    Cell = cell;
    GameBoard = gameBoard;
    Data = data;
  }
}
