namespace GameplayAbilitySystem;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AttributeSetData : Resource {
  [Export] public Array<AttributeDefault>? AttributeDefaults { get; private set; }
}