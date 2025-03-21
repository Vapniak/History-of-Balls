namespace HOB.GameEntity;

using GameplayAbilitySystem;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class Entity : Node {
  public EntityBody Body { get; private set; }
  public string EntityName { get; private set; }

  [Notify]
  public GameCell Cell {
    get => _cell.Get();
    set => _cell.Set(value);
  }

  public IEntityManagment EntityManagment { get; private set; }
  // public EntityUI EntityUI { get; set; }
  public GameplayAbilitySystem AbilitySystem { get; private set; }

  [Notify]
  private IMatchController OwnerController { get => _ownerController.Get(); set => _ownerController.Set(value); }

  public Entity(string name, GameCell cell, IEntityManagment entityManagment, Array<GameplayAttributeSet> attributeSets, Array<GameplayAbilityResource> abilities, EntityBody body) {
    Cell = cell;
    EntityManagment = entityManagment;
    EntityName = name;

    Body = body;
    AddChild(Body);

    AbilitySystem = new();
    // AbilitySystem.InitAbilityOwnerInfo(this);
    if (attributeSets != null) {
      foreach (var @as in attributeSets) {
        AbilitySystem.AddAttributeSet(@as);
      }
    }

    if (abilities != null) {
      foreach (var ability in abilities) {
        AbilitySystem.GrantAbility(ability.CreateInstance(AbilitySystem));
      }
    }

    CallDeferred(MethodName.SetPosition, Cell.GetRealPosition());
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

  public void SetMaterialOverlay(Material material) {
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
