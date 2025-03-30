namespace HOB;

using GameplayTags;

public enum HOBTags {
  [GameplayTag("Ability")] Ability,
  [GameplayTag("Ability.Move")] AbilityMove,
  [GameplayTag("Ability.Attack")] AbilityAttack,
  [GameplayTag("Ability.Capture")] AbilityCapture,
  [GameplayTag("Ability.ProcessResources")] AbilityProcessResources,

  [GameplayTag("Effect.Processing")] EffectProcessing,

  [GameplayTag("Cooldown")] Cooldown,
  [GameplayTag("Cooldown.Move")] CooldownMove,
  [GameplayTag("Cooldown.Attack")] CooldownAttack,

  [GameplayTag("State")] State,
  [GameplayTag("State.CurrentTurn")] StateCurrentTurn,
  [GameplayTag("State.Dead")] StateDead,
  [GameplayTag("State.Block.Movement")] StateBlockMovement,

  [GameplayTag("Entity.Type")] EntityType,
  [GameplayTag("Entity.Type.Unit")] EntityTypeUnit,
  [GameplayTag("Entity.Type.Structure")] EntityTypeStructure,
  [GameplayTag("Entity.Type.Unit.Ranged")] EntityTypeUnitRanged,
  [GameplayTag("Entity.Type.Unit.Melee")] EntityTypeUnitMelee,


  [GameplayTag("Event")] Event,
  [GameplayTag("Event.ResourceGenerated")] EventResourceGenerated,
  [GameplayTag("Event.Entity.Capture")] EventEntityCapture,
  [GameplayTag("Event.Turn")] EventTurn,
  [GameplayTag("Event.Turn.Started")] EventTurnStarted,
  [GameplayTag("Event.Turn.Preparation")] EventTurnPreparation,
  [GameplayTag("Event.Turn.Ended")] EventTurnEnded,
  [GameplayTag("Event.Game.Started")] EventGameStarted,
  [GameplayTag("Event.Game.Ended")] EventGameEnded,
}