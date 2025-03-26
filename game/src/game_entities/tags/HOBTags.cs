namespace HOB;

using GameplayTags;

public enum HOBTags {
  [GameplayTag("Ability")] Ability,
  [GameplayTag("Ability.Move")] Move,
  [GameplayTag("Ability.Attack")] Attack,

  [GameplayTag("Cooldown")] Cooldown,
  [GameplayTag("Cooldown.Move")] CooldownMove,
  [GameplayTag("Cooldown.Attack")] CooldownAttack,

  [GameplayTag("State")] State,
  [GameplayTag("State.Dead")] StateDead,
  [GameplayTag("State.Block.Movement")] StateBlockMovement,

  [GameplayTag("Entity.Type")] EntityType,
  [GameplayTag("Entity.Type.Unit")] EntityTypeUnit,
  [GameplayTag("Entity.Type.Structure")] EntityTypeStructure,
  [GameplayTag("Entity.Type.Unit.Ranged")] EntityTypeUnitRanged,
  [GameplayTag("Entity.Type.Unit.Melee")] EntityTypeUnitMelee,

  [GameplayTag("Resource")] Resource,
  [GameplayTag("Resource.Type")] ResourceType,
  [GameplayTag("Resource.Type.Primary")] ResourceTypePrimary,
  [GameplayTag("Resource.Type.Secondary")] ResourceTypeSecondary,
}