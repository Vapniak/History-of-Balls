namespace GameplayAbilitySystem;

using Godot;
using Godot.Collections;

public partial class Aggregator : RefCounted {
  private Dictionary<AttributeModifierType, Array<AggregatorMod>> Mods { get; set; } = new();

  public void AddMod(AttributeModifierType type, float magnitude) {
    if (Mods.TryGetValue(type, out var value)) {
      value.Add(new(magnitude));
    }
    else {
      Mods.Add(type, new() { new(magnitude) });
    }
  }

  public float Evaluate(float value) {
    var sum = SumMods();

    var multiply = 1f;
    if (Mods.TryGetValue(AttributeModifierType.Multiply, out var multi)) {
      multiply = MultiplyMods(multi);
    }

    float? @override = null;
    if (Mods.TryGetValue(AttributeModifierType.Override, out var over)) {
      @override = OverrideMods(over);
    }

    value = (value + sum) * multiply;
    if (@override != null) {
      value = @override.GetValueOrDefault();
    }

    return value;
  }

  public float SumMods() {
    var sum = 0f;
    if (Mods.TryGetValue(AttributeModifierType.Add, out var add)) {
      foreach (var mod in add) {
        sum += mod.Magnitude;
      }
    }

    return sum;
  }

  private static float MultiplyMods(Array<AggregatorMod> mods) {
    var multiply = 1f;

    foreach (var mod in mods) {
      multiply *= mod.Magnitude;
    }

    return multiply;
  }

  private static float? OverrideMods(Array<AggregatorMod> mods) {
    float? @override = null;

    foreach (var mod in mods) {
      @override = mod.Magnitude;
    }

    return @override;
  }
}