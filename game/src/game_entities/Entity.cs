namespace HOB.GameEntity;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using HexGridMap;
using System.Threading.Tasks;

[GlobalClass]
public partial class Entity : Node, ITurnAware {
  public EntityBody Body { get; private set; }
  public string EntityName { get; private set; }

  [Notify]
  public GameCell Cell {
    get => _cell.Get();
    set => _cell.Set(value);
  }

  public IEntityManagment EntityManagment { get; private set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; private set; }

  [Notify]
  private IMatchController? OwnerController { get => _ownerController.Get(); set => _ownerController.Set(value); }

  public Entity(
    string name,
    GameCell cell,
    IEntityManagment entityManagment,
    Array<GameplayAttributeSet>? attributeSets,
    Array<GameplayAbilityResource>? abilities,
    TagContainer? tags,
    EntityBody body,
    IMatchController? owner
    ) {
    EntityManagment = entityManagment;
    EntityName = name;
    OwnerController = owner;
    AbilitySystem = new();
    Body = body;

    TreeEntered += () => {
      Cell = cell;

      AddChild(Body);

      AddChild(AbilitySystem);
      AbilitySystem.Owner = this;

      AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;
      if (tags != null) {
        AbilitySystem.OwnedTags.AddTags(tags);
      }

      if (attributeSets != null) {
        foreach (var @as in attributeSets) {
          AbilitySystem.AttributeSystem.AddAttributeSet(@as);
        }
      }

      if (abilities != null) {
        foreach (var ability in abilities) {
          AbilitySystem.CallDeferred(nameof(AbilitySystem.GrantAbility), ability.CreateInstance(AbilitySystem));
        }
      }

      Body.AddChild(EntityUi3D.Create(this));

      CallDeferred(MethodName.SetPosition, Cell.GetRealPosition());
    };
  }

  public override void _Ready() {
    base._Ready();

  }

  public override void _ExitTree() {
    base._ExitTree();

    AbilitySystem.CancelAllAbilities();
  }

  public void SetPosition(Vector3 position) {
    Body.GlobalPosition = position;
  }

  public bool TryGetOwner(out IMatchController? owner) {
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

  public void ChangeOwner(IMatchController? newOwner) {
    if (newOwner != OwnerController) {
      OwnerController = newOwner;
    }
  }

  public void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    if (AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var healthAttributeSet)) {
      if (attribute == healthAttributeSet?.HealthAttribute) {
        if (newValue <= 0f) {
          AbilitySystem.OwnedTags.AddTag(TagManager.GetTag(HOBTags.StateDead));
          QueueFree();
        }
      }
    }
  }

  public bool IsCurrentTurn() => TryGetOwner(out var owner) && owner != null && owner.IsCurrentTurn();
}
