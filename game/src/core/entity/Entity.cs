namespace HOB.GameEntity;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using System.Threading.Tasks;

[GlobalClass]
public partial class Entity : Node3D, ITurnAware {
  public EntityBody Body { get; private set; }
  public string EntityName { get; private set; }

  public Texture2D? Icon => GameAssetsRegistry.Instance.GetIconFor(this);

  [Notify]
  public GameCell Cell {
    get => _cell.Get();
    set => _cell.Set(value);
  }

  public IEntityManagment EntityManagment { get; private set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; private set; }

  public EntityUIWidget UI { get; private set; }

  [Notify]
  public IMatchController? OwnerController { get => _ownerController.Get(); set => _ownerController.Set(value); }

  public Entity(
    string name,
    GameCell cell,
    IEntityManagment entityManagment,
    AttributeSetsContainer? attributeSets,
    Array<GameplayAbility>? abilities,
    TagContainer? tags,
    EntityBody body,
    IMatchController? owner
    ) {
    EntityManagment = entityManagment;
    EntityName = name;
    OwnerController = owner;
    AbilitySystem = new();
    Body = body;
    Cell = cell;


    Scale = Vector3.One * Mathf.Epsilon;
    Position = Cell.GetRealPosition();

    Ready += () => {
      var tween = CreateTween();

      tween.TweenProperty(this, "scale", Vector3.One, 1f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);
    };

    TreeEntered += () => {

      AddChild(Body);

      AddChild(AbilitySystem);
      AbilitySystem.Owner = this;

      AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;
      if (tags != null) {
        AbilitySystem.OwnedTags.AddTags(tags);
      }

      if (attributeSets?.AttributeSets != null) {
        foreach (var @as in attributeSets.AttributeSets) {
          AbilitySystem.AttributeSystem.AddAttributeSet(@as);
        }
      }

      if (abilities != null) {
        foreach (var ability in abilities) {
          AbilitySystem.CallDeferred(nameof(AbilitySystem.GrantAbility), ability.CreateInstance(AbilitySystem));
        }
      }

      if (!AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeProp))) {
        var ui3D = EntityUi3D.Create(this);
        UI = ui3D.EntityUI;
        AddChild(ui3D);
      }
    };

    OwnerControllerChanging += AbilitySystem.CancelAllAbilities;
  }

  public override void _ExitTree() {
    base._ExitTree();

    AbilitySystem.CancelAllAbilities();
  }

  public bool TryGetOwner(out IMatchController? owner) {
    owner = OwnerController;
    return owner != null;
  }

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

  public void Die() {
    AbilitySystem.OwnedTags.AddTag(TagManager.GetTag(HOBTags.StateDead));
    var tween = CreateTween();
    tween.TweenProperty(this, "scale", Vector3.One * Mathf.Epsilon, 1f).SetTrans(Tween.TransitionType.Back);
    tween.Finished += QueueFree;
  }

  public void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    if (AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var healthAttributeSet)) {
      if (attribute == healthAttributeSet?.HealthAttribute) {
        if (newValue <= 0f) {
          Die();
        }
      }
    }
  }

  public bool IsCurrentTurn() => TryGetOwner(out var owner) && owner != null && owner.IsCurrentTurn();
}
