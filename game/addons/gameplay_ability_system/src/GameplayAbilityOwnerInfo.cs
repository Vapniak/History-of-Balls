namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayAbilityOwnerInfo : RefCounted {
  public Node OwnerNode { get; private set; }
  public GameplayAbilitySystem AbilitySystem { get; private set; }

  public GameplayAbilityOwnerInfo(Node ownerNode, GameplayAbilitySystem abilitySystem) {
    OwnerNode = ownerNode;
    AbilitySystem = abilitySystem;
  }
}