namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class HealthAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute HealthAttribute { get; private set; }
  [Export] public GameplayAttribute MaxHealthAttribute { get; private set; }

  public override void PreAttributeChange(AttributeSystem attributeSystem, GameplayAttribute attribute, ref float newValue) {
    base.PreAttributeChange(attributeSystem, attribute, ref newValue);

    // TODO: adjust for max
    // if (attribute == MaxHealthAttribute) {
    //   var value = attributeSystem.GetAttributeCurrentValue(HealthAttribute).GetValueOrDefault();
    //   AdjustForMax(ref value, attributeSystem.GetAttributeCurrentValue(MaxHealthAttribute).GetValueOrDefault(), newValue);

    //   // FIXME: idk if this should be base value or the gameplay ability system should calculate current values like this
    //   attributeSystem.SetAttributeCurrentValue(HealthAttribute, value);
    // }
  }
  public override void PostGameplayEffectExecute(GameplayEffectModData data) {
    base.PostGameplayEffectExecute(data);

    var @as = data.Target.AttributeSystem;
    if (data.EvaluatedData.Attribute == HealthAttribute) {
      var baseValue = @as.GetAttributeBaseValue(HealthAttribute).GetValueOrDefault();
      var maxHealth = @as.GetAttributeBaseValue(MaxHealthAttribute).GetValueOrDefault();
      //@as.SetAttributeBaseValue(HealthAttribute, Mathf.Clamp(baseValue, 0, maxHealth));

      // TODO: this sould be gameplay cue
      if (data.Target.GetOwner() is Entity entity) {
        // TODO: fixme only add is shown
        var magnitude = data.EvaluatedData.Magnitude;
        var text = FloatingText3D.Create();
        text.Label?.PushColor(Colors.Red);
        text.Label?.AppendText($"{magnitude} Health");
        GameInstance.GetWorld().AddChild(text);
        text.GlobalPosition = entity.GlobalPosition + Vector3.Up * 2;
        _ = text.Animate();
      }
    }
  }

  public override GameplayAttribute[] GetAttributes() {
    return new[] { HealthAttribute, MaxHealthAttribute };
  }
}
