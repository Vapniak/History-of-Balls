namespace HOB;

using GameplayTags;

public enum HOBTags {
  [GameplayTag("Ability")] Ability,
  [GameplayTag("Ability.Move")] AbilityMove,
  [GameplayTag("Ability.Attack")] AbilityAttack,
  [GameplayTag("Ability.Capture")] AbilityCapture,
  [GameplayTag("Ability.ProcessResources")] AbilityProcessResources,

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
  [GameplayTag("Entity.Type.Structure.Village")] EntityTypeStructureVillage,
  [GameplayTag("Entity.Type.Structure.City")] EntityTypeStructureCity,
  [GameplayTag("Entity.Type.Structure.Factory")] EntityTypeStructureFactory,

  [GameplayTag("Entity.Type.Unit.Ranged")] EntityTypeUnitRanged,
  [GameplayTag("Entity.Type.Unit.Infantry")] EntityTypeUnitInfantry,
  [GameplayTag("Entity.Type.Unit.Cavalry")] EntityTypeUnitCavalry,
  [GameplayTag("Entity.Type.Prop")] EntityTypeProp,


  [GameplayTag("Event")] Event,
  [GameplayTag("Event.ResourceGenerated")] EventResourceGenerated,
  [GameplayTag("Event.Entity.Capture")] EventEntityCapture,
  [GameplayTag("Event.Turn")] EventTurn,
  [GameplayTag("Event.Turn.Started")] EventTurnStarted,
  [GameplayTag("Event.Turn.Preparation")] EventTurnPreparation,
  [GameplayTag("Event.Turn.Ended")] EventTurnEnded,
  [GameplayTag("Event.Game.Started")] EventGameStarted,
  [GameplayTag("Event.Game.Ended")] EventGameEnded,

  [GameplayTag("GameplayCue")] GameplayCue,
  [GameplayTag("GameplayCue.Move.Dust")] GameplayCueMoveDust,
  [GameplayTag("GameplayCue.Sparkles")] GameplayCueSparkles,
}