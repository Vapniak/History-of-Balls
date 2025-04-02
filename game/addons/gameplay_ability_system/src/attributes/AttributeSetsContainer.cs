namespace GameplayAbilitySystem;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AttributeSetsContainer : Resource {
  [Export] public Array<GameplayAttributeSet>? AttributeSets { get; private set; }
}