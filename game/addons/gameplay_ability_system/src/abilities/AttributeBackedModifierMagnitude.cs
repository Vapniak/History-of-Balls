namespace GameplayAbilitySystem;


using Godot;

[GlobalClass]
public partial class AttributeBackedModifierMagnitude : ModifierMagnitudeResource {
  [Export] public Curve? ScalingFunction { get; private set; }

  [Export] public GameplayAttribute? CaptureAttribute { get; private set; }
  [Export] public CaptureAttributeFrom CaptureAttributeFrom { get; private set; }
  [Export] public CaptureAttributeWhen CaptureAttributeWhen { get; private set; }

  private float? CreationAttributeValue { get; set; }

  public override void Initialize(GameplayEffectInstance effectInstance) {
    CreationAttributeValue = GetAttribueValue(effectInstance);
  }
  public override float CalculateMagnitude(GameplayEffectInstance effectInstance) {
    var value = GetCaptureAttribute(effectInstance);
    if (value != null) {
      if (ScalingFunction != null) {
        return ScalingFunction.Sample(value.GetValueOrDefault());
      }
    }


    return value.GetValueOrDefault();
  }

  private float? GetCaptureAttribute(GameplayEffectInstance gameplayEffectInstance) {
    if (CaptureAttributeWhen == CaptureAttributeWhen.OnApplication) {
      return GetAttribueValue(gameplayEffectInstance);
    }

    if (CaptureAttributeWhen == CaptureAttributeWhen.OnCreation) {
      return CreationAttributeValue;
    }

    return null;
  }

  private float? GetAttribueValue(GameplayEffectInstance instance) {
    if (CaptureAttribute != null) {
      switch (CaptureAttributeFrom) {
        case CaptureAttributeFrom.Source:
          return instance.Source.GetAttributeCurrentValue(CaptureAttribute);
        case CaptureAttributeFrom.Target:
          return instance.Target?.GetAttributeCurrentValue(CaptureAttribute);
        default:
          break;
      }
    }

    return null;
  }
}

public enum CaptureAttributeFrom {
  Source,
  Target
}

public enum CaptureAttributeWhen {
  OnCreation,
  OnApplication
}
