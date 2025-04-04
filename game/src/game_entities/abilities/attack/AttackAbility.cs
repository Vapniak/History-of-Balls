namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[GlobalClass]
public abstract partial class AttackAbility : HOBAbilityResource {
  [Export] public GameplayEffectResource? DamageEffect { get; private set; }
  [Export] public GameplayEffectResource? BlockMovementEffect { get; private set; }

  public partial class Instance : HOBEntityAbilityInstance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is AttackTargetData data) {
        return GetAttackableEntities().entities.Contains(data.TargetAbilitySystem.GetOwner<Entity>()) && base.CanActivateAbility(eventData);
      }

      return base.CanActivateAbility(eventData);
    }
    public virtual (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
      var attackableEntities = new List<Entity>();
      var cellsInR = new List<GameCell>();

      var cells = OwnerEntity.Cell.GetCellsInRange(GetRange());
      foreach (var cell in cells) {
        if (cell == OwnerEntity.Cell) {
          continue;
        }

        cellsInR.Add(cell);
        var entities = OwnerEntity.EntityManagment.GetEntitiesOnCell(cell);
        attackableEntities.AddRange(entities.Where(CanBeAttacked));
      }

      return (attackableEntities.ToArray(), cellsInR.ToArray());
    }

    public virtual bool CanBeAttacked(Entity entity) {
      return entity.TryGetOwner(out var enemyOwner) && OwnerEntity.TryGetOwner(out var owner) && enemyOwner != owner && entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit));
    }

    protected void ShowDamageNumber(Vector3 pos) {
      var damageEffect = (AbilityResource as AttackAbility)?.DamageEffect;
      if (damageEffect?.EffectDefinition?.Modifiers == null) {
        return;
      }

      var ei = OwnerAbilitySystem.MakeOutgoingInstance(damageEffect, Level);
      foreach (var modifier in damageEffect.EffectDefinition.Modifiers) {
        var magnitude = modifier.GetMagnitude(ei);
        var text = FloatingText.Create();
        text.Label?.PushColor(Colors.Red);
        text.Label?.AppendText($"{magnitude} {modifier?.Attribute?.AttributeName}");
        GameInstance.GetWorld().AddChild(text);
        text.GlobalPosition = pos + Vector3.Up * 2;
        _ = text.Animate();
      }
    }

    protected virtual uint GetRange() {
      if (OwnerEntity.AbilitySystem.AttributeSystem.TryGetAttributeSet<AttackAttributeSet>(out var attributeSet)) {
        return (uint)OwnerEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attributeSet.Range).GetValueOrDefault();
      }
      else {
        Debug.Assert(false, "Entity doesnt have attack attribute set");
        return 0;
      }
    }
  }
}
